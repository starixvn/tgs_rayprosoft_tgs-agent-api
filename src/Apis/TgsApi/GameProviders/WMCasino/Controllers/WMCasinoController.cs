using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings.WMCasino;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Controllers
{
    [Route("api/wm")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.WMCasino)]
    public class WMCasinoController : ControllerBase
    {
        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("betdetail")]
        public async Task<ErrorResponse> BetDetailsAsync(
            [FromServices] IBrandOperations brandOperations,
            [FromServices] IPlayerOperations playerOperations,
            [FromServices] IWMCasinoPlayerServices playerServices,
            [FromServices] IWMDataStore store,
            [FromServices] IWMCasinoActionModelMapper map,
            BetDetailRequest request)
        {
            var agent = HttpContext.Items.Get<WMCasinoAgent>(WMCasinoConsts.ContextItems.Agent, null);
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

            var configurationIds = new ConcurrentDictionary<string, Guid>();
            var betDetails = new List<BetDetail>();

            foreach (var r in request.Results)
            {
                var playerCode = playerServices.GetPlayerCode(r.User, agent!.Prefix, agent!.Suffix);

                if (!configurationIds.TryGetValue(playerCode, out var gameProviderConfigurationId))
                {
                    var brandId = await playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                    var bgpc = await brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId)
                        .ConfigureAwait(false);

                    gameProviderConfigurationId = bgpc.GameProviderConfigurationId;
                    configurationIds[playerCode] = gameProviderConfigurationId;
                }

                var betDetail = new BetDetail(
                    gameProviderId,
                    gameProviderConfigurationId,
                    playerCode,
                    r);

                betDetails.Add(betDetail);
            }

            await store
                .AddBetResultsToQueueAsync(betDetails)
                .ConfigureAwait(false);

            return map.Response(GameApiError.Success, string.Empty);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("canceltransfer")]
        public async ValueTask<CancelWalletTransferResponse> CancelWalletTransferAsync(
            [FromServices] IWMCasinoBetActionServices services,
            [FromServices] IWMCasinoActionModelMapper map,
            CancelWalletTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.ActionType == WalletActionType.Debit)
            {
                var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

                var gameProviderConfigurationId = HttpContext.Items.Get(
                    Consts.ContextItems.GameProviderConfigurationId,
                    Guid.Empty);

                var betResult = await services
                    .CancelLockAmountAsync(gameProviderId, gameProviderConfigurationId, playerCode, request)
                    .ConfigureAwait(false);

                return map.Response(request, betResult.Balance);
            }

            var balanceResult = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);

            return map.Response(request, balanceResult.Balance);
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<ErrorResponse> EntranceAsync([FromForm] BaseRequest request)
            => ValueTask.FromResult(new ErrorResponse());

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("gamerecord")]
        public ValueTask<GameProviderGameRecordResponse> GetGameRecordAsync(
            [FromServices] IWMCasinoGameProviderHistoryServices services,
            GameProviderGameRecordRequest request)
            => services.GetGameRecordAsync(request);

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("balance")]
        public async ValueTask<GetBalanceResponse> PlayersBalanceAsync(
            [FromServices] IWMCasinoBetActionServices services,
            [FromServices] IWMCasinoActionModelMapper map,
            GetBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);

            return map.Response(request.User, r);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("transfer")]
        public async ValueTask<WalletTransferResponse> WalletTransferAsync(
            [FromServices] IWMCasinoBetActionServices services,
            [FromServices] IWMCasinoActionModelMapper map,
            WalletTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.ActionType == WalletActionType.Debit)
            {
                var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

                var gameProviderConfigurationId = HttpContext.Items.Get(
                    Consts.ContextItems.GameProviderConfigurationId,
                    Guid.Empty);

                var betResult = await services
                    .LockAmountAsync(gameProviderId, gameProviderConfigurationId, playerCode, request)
                    .ConfigureAwait(false);

                return map.Response(request, betResult.Balance);
            }

            var balanceResult = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);

            return map.Response(request, balanceResult.Balance);
        }
    }
}

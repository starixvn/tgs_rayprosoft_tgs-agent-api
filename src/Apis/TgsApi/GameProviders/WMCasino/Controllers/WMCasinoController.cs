using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
using TGS.Core.Infrastructures.Extensions;
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
        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<ErrorResponse> Entrance([FromForm] BaseRequest request)
            => ValueTask.FromResult(new ErrorResponse());

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
                    Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

                var betResult = await services
                    .LockAmountAsync(gameProviderId, gameProviderConfigurationId, playerCode, request)
                    .ConfigureAwait(false);
                return map.Response(request, betResult.Balance);
            }

            var balanceResult = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return map.Response(request, balanceResult.Balance);
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
                    Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

                var betResult = await services
                    .CancelLockAmountAsync(gameProviderId, gameProviderConfigurationId, playerCode, request)
                    .ConfigureAwait(false);
                return map.Response(request, betResult.Balance);
            }

            var balanceResult = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return map.Response(request, balanceResult.Balance);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("betdetail")]
        public async Task<ErrorResponse> BetDetailsAsync(
            [FromServices] IBrandOperations brandOperations,
            [FromServices] IPlayerOperations playerOperations,
            [FromServices] IWMCasinoBetActionServices services,
            [FromServices] IWMCasinoPlayerServices playerServices,
            [FromServices] IWMCasinoActionModelMapper map,
            BetDetailRequest request)
        {
            var agent = HttpContext.Items.Get<WMCasinoAgent>(WMCasinoConsts.ContextItems.Agent, null);
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

            var results = (from r in request.Results
                           group r by r.User
                           into g
                           select g)
                .ToDictionary(g => g.Key, g => g.ToArray());

            var configurationIds = new ConcurrentDictionary<Guid, Guid>();

            List<ExecuteResult> executeResults = new();

            foreach (var r in results)
            {
                var playerCode = playerServices.GetPlayerCode(r.Key, agent!.Prefix, agent!.Suffix);
                var brandId = await playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

                if (!configurationIds.TryGetValue(brandId, out var gameProviderConfigurationId))
                {
                    var bgpc = await brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId)
                        .ConfigureAwait(false);

                    gameProviderConfigurationId = bgpc.GameProviderConfigurationId;
                    configurationIds[brandId] = gameProviderConfigurationId;
                }

                ErrorResponse errorResponse;

                try
                {
                    await services.ExecuteBetResultsByRoundAsync(
                            gameProviderId, gameProviderConfigurationId, playerCode, r.Value, false)
                        .ConfigureAwait(false);

                    errorResponse = map.Response(GameApiError.Success, string.Empty);
                }
                catch (Exception ex)
                {
                    errorResponse = map.Response(GameApiError.SystemError, ex.Message);
                }

                executeResults.Add(new ExecuteResult(playerCode, errorResponse));
            }

            var message = executeResults
                .Where(r => r.Response.ErrorCode != (int)GameApiError.Success)
                .Select(r => $"user {r.PlayerCode} with error: {r.Response.ErrorMessage}")
                .ToArray().JoinAsStringByComma();

            return message.IsNullOrEmpty()
                ? map.Response(GameApiError.Success, string.Empty)
                : map.Response(GameApiError.SystemError, message);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("gamerecord")]
        public ValueTask<GameProviderGameRecordResponse> GetGameRecordAsync(
            [FromServices] IWMCasinoGameProviderHistoryServices services,
            GameProviderGameRecordRequest request)
            => services.GetGameRecordAsync(request);

        private record ExecuteResult(string PlayerCode, ErrorResponse Response);
    }
}
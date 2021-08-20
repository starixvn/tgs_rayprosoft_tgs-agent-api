using System;
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
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;
using BetResult = TGS.Shared.Game.Models.WMCasino.Incoming.BetResult;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Controllers
{
    [Route("api/wm")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.WMCasino)]
    public class WMCasinoController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly IBrandOperations _brandOperations;
        private readonly IWMCasinoActionModelMapper _map;
        private readonly IPlayerOperations _playerOperations;
        private readonly IWMCasinoServices _services;
        private readonly IWMCasinoUtility _utility;

        public WMCasinoController(
            IGamePlayActions actions,
            IWMCasinoServices services,
            IWMCasinoActionModelMapper map,
            IWMCasinoUtility utility,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations)
        {
            _actions = actions;
            _services = services;
            _map = map;
            _utility = utility;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<ErrorResponse> Entrance([FromForm] BaseRequest request)
            => ValueTask.FromResult(new ErrorResponse());

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("balance")]
        public async ValueTask<GetBalanceResponse> PlayersBalanceAsync(GetBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return _map.Response(request.User, r);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("transfer")]
        public async ValueTask<WalletTransferResponse> WalletTransferAsync(WalletTransferRequest request)
        {
            if (request.ActionType == WalletActionType.Debit)
            {
                return await LockAmountAsync(request).ConfigureAwait(false);
            }

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("canceltransfer")]
        public async ValueTask<CancelWalletTransferResponse> CancelWalletTransferAsync(
            CancelWalletTransferRequest request)
        {
            if (request.ActionType == WalletActionType.Debit)
            {
                return await CancelLockAmountAsync(request).ConfigureAwait(false);
            }

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("betdetail")]
        public async Task<ErrorResponse> BetDetailsAsync(BetDetailRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

            var results = (from r in request.Results
                           group r by r.PlayerCode
                           into g
                           select g)
                .ToDictionary(g => g.Key, g => g.ToArray());

            var configurationIds = new Dictionary<Guid, Guid>();

            var tasks = results.Select(async r =>
            {
                var playerCode = r.Key;
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

                if (configurationIds.TryGetValue(brandId, out var gameProviderConfigurationId))
                {
                    return new
                    {
                        PlayerCode = playerCode,
                        Response = await ExecuteBetResultsByRoundAsync(
                                gameProviderId, gameProviderConfigurationId, playerCode, r.Value)
                            .ConfigureAwait(false)
                    };
                }

                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId)
                    .ConfigureAwait(false);

                gameProviderConfigurationId = bgpc.GameProviderConfigurationId;
                configurationIds[brandId] = gameProviderConfigurationId;

                return new
                {
                    PlayerCode = playerCode,
                    Response = await ExecuteBetResultsByRoundAsync(
                            gameProviderId, gameProviderConfigurationId, playerCode, r.Value)
                        .ConfigureAwait(false)
                };
            });

            var responses = await Task.WhenAll(tasks);

            var message = responses
                .Where(r => r.Response.ErrorCode != (int)GameApiError.Success)
                .Select(r => $"user {r.PlayerCode} with error: {r.Response.ErrorMessage}")
                .ToArray().JoinAsStringByComma();

            return message.IsNullOrEmpty()
                ? _map.Response(GameApiError.Success, string.Empty)
                : _map.Response(GameApiError.SystemError, message);
        }

        private async ValueTask<WalletTransferResponse> LockAmountAsync(WalletTransferRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = HttpContext.Items.Get(
                Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync(
                    gameProviderId, gameProviderConfigurationId, playerCode, request.ExternalGameId)
                .ConfigureAwait(false);

            var betData = _map.LockAmountRequest(playerCode, data, request);
            var r = await _actions.LockAmountAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }

        private async ValueTask<CancelWalletTransferResponse> CancelLockAmountAsync(CancelWalletTransferRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = HttpContext.Items.Get(
                Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync(gameProviderId,
                    gameProviderConfigurationId, playerCode, request.ExternalGameId)
                .ConfigureAwait(false);

            var betData = _map.CancelLockAmountRequest(playerCode, data, request);
            var r = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }

        private async ValueTask<ErrorResponse> ExecuteBetResultsByRoundAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, string playerCode, BetResult[] betResults, bool isManual = false)
        {
            var data = await _services
                .GetGameProviderConfigurationDataWithDesktopPlatformAsync(gameProviderId, gameProviderConfigurationId)
                .ConfigureAwait(false);

            await PlaceAndSettleBetAsync(data, playerCode, betResults, isManual);

            var results = betResults.Where(b => b.Reset.IgnoreCaseEquals("Y")).ToArray();

            if (results.Length > 0)
            {
                await ResettleOrVoidRoundAsync(data, playerCode, results, isManual);
            }

            return _map.Response(GameApiError.Success, string.Empty);
        }

        private async ValueTask PlaceAndSettleBetAsync(GameProviderConfigurationData data,
            string playerCode, BetResult[] betResults, bool isManual)
        {
            var betResult = betResults.First();
            var externalGameActionGroupId =
                _utility.GetExternalRoundIdByBetResult(betResult.RoundId, betResult.SubRoundId);

            var externalIds =
                await _services.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(data.GameProviderId,
                    playerCode, externalGameActionGroupId).ConfigureAwait(false);

            var betData = _map.PlaceAndSettleBetRequest(data, playerCode, betResults, externalIds, isManual);

            if (betData != null)
            {
                await _actions.PlaceAndSettleBetAsync(betData).ConfigureAwait(false);
            }
        }

        private async ValueTask ResettleOrVoidRoundAsync(GameProviderConfigurationData data,
            string playerCode, BetResult[] betResults, bool isManual)
        {
            var betResult = betResults.First();
            var externalGameActionGroupId =
                _utility.GetExternalRoundIdByBetResult(betResult.RoundId, betResult.SubRoundId);

            var externalIds =
                await _services.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(data.GameProviderId,
                    playerCode, externalGameActionGroupId).ConfigureAwait(false);

            var groupResults = (from r in betResults
                                group r by r.GameResult.IgnoreCaseContains("cancel")
                                into g
                                select g).ToDictionary(g => g.Key, g => g.ToArray());

            if (groupResults.TryGetValue(false, out var resettleResults) && resettleResults.Length > 0)
            {
                await ResettleAsync(data, playerCode, betResults, externalIds, isManual);
            }

            if (groupResults.TryGetValue(true, out var voidResults) && voidResults.Length > 0)
            {
                await VoidRoundAsync(data, playerCode, betResults, externalIds, isManual);
            }
        }

        private async ValueTask ResettleAsync(GameProviderConfigurationData data, string playerCode,
            BetResult[] betResults, GameActionExternalIds[] externalIds, bool isManual)
        {
            var (cancelData, settleData) = _map.ResettleRequest(data, playerCode, betResults, externalIds, isManual);

            if (cancelData != null)
            {
                await _actions.CancelTransactionAsync(cancelData).ConfigureAwait(false);
            }

            if (settleData != null)
            {
                await _actions.SettleBetAsync(settleData).ConfigureAwait(false);
            }
        }

        private ValueTask VoidRoundAsync(GameProviderConfigurationData data, string playerCode,
            BetResult[] betResults, GameActionExternalIds[] externalIds, bool isManual)
            => throw new NotImplementedException();
    }
}
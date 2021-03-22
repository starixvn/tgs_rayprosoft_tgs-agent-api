// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(GameProviderConst.SBOBET)]
    public sealed class SBOBETGameController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly ISBOBETActionModelMapper _map;
        private readonly ISBOBETServices _services;
        private readonly ISettingOperations _settingOperations;

        public SBOBETGameController(
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations)
        {
            _actions = actions;
            _map = map;
            _services = services;
            _settingOperations = settingOperations;
        }

        [HttpPost]
        [Route("GetBalance")]
        public async ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var r = await _actions.GetBalanceAsync(GameProviderConst.SBOBET, request.PlayerCode)
                .ConfigureAwait(false);

            return _map.Response(request.PlayerCode, r);
        }

        [HttpPost]
        [Route("Deduct")]
        public async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var configuration =
                await _settingOperations
                    .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET)
                    .ConfigureAwait(false);

            if (configuration.DelaySecondsForPlaceBet > 0)
            {
                await Task.Delay(configuration.DelaySecondsForPlaceBet * 1000).ConfigureAwait(false);
            }

            var data = await _services.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);

            var betData = _map.Request(data, request);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r);
        }

        [HttpPost]
        [Route("Settle")]
        public async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var actionTypes = new[] {WinBet, LoseBet, TieBet, FreeBet};

            var data = await _services.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);
            var gameProviderId = data.GameProviderId;
            var externalRoundId = request.ExternalTransactionId;
            var playerCode = request.PlayerCode;

            var (roundStatus, gameId) =
                await _services.GetRoundStatusAndGameIdAsync(gameProviderId, data, playerCode, externalRoundId,
                    request.ExternalGameId, request.GameType).ConfigureAwait(false);

            var gameActionExternalIds = await _services
                .GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId, actionTypes)
                .ConfigureAwait(false);

            var betData = _map.Request(data, externalRoundId, request, roundStatus, gameActionExternalIds);
            var r = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r);
        }

        /// <summary>
        ///     case: Cancel Bet or Void Round
        /// </summary>
        [HttpPost]
        [Route("Cancel")]
        public async ValueTask<CancelBetResponse> CancelBetAsync(CancelBetRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);
            Func<GameProviderConfigurationData, CancelBetRequest, ValueTask<CancelBetResponse>> func =
                request.IsCancelAll ? VoidRoundAsync : CancelBetAsync;

            return await func(data, request).ConfigureAwait(false);
        }

        /// <summary>
        ///     case: Cancel Settle or Cancel CancelTransaction
        /// </summary>
        [HttpPost]
        [Route("Rollback")]
        public async ValueTask<RollbackResponse> RollbackAsync(RollbackRequest request)
        {
            var actionTypes = new[] {WinBet, LoseBet, TieBet, FreeBet, CancelTransaction};

            var data = await _services.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);
            var playerCode = request.PlayerCode;
            var externalRoundId = request.ExternalTransactionId;
            var gameProviderId = data.GameProviderId;

            var (_, gameId) =
                await _services.GetRoundStatusAndGameIdAsync(gameProviderId, data, playerCode, externalRoundId,
                    request.ExternalGameId, request.GameType).ConfigureAwait(false);

            var gameActionExternalIds =
                await _services
                    .GetGameActionExternalIdsAsync(gameProviderId, request.PlayerCode, externalRoundId, gameId,
                        actionTypes).ConfigureAwait(false);

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r);
        }

        [HttpPost]
        [Route("GetBetStatus")]
        public async ValueTask<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);
            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;
            var gameProviderId = data.GameProviderId;

            var (_, gameId) =
                await _services.GetRoundStatusAndGameIdAsync(gameProviderId, data, playerCode, externalRoundId,
                    request.ExternalGameId, request.GameType).ConfigureAwait(false);

            var ga = await _services
                .GetValidGameActionsAsync(gameProviderId, request.PlayerCode, request.ExternalRoundId, gameId)
                .ConfigureAwait(false);

            if (ga == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalTransactionId} and transactionId: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            return _map.Response(request, ga);
        }

        #region Private Methods

        private async ValueTask<CancelBetResponse> CancelBetAsync(GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var betData = _map.Request(data, request);
            var r = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r);
        }

        private async ValueTask<CancelBetResponse> VoidRoundAsync(GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var gameProviderId = data.GameProviderId;
            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;

            var (roundStatus, gameId) =
                await _services.GetRoundStatusAndGameIdAsync(gameProviderId, data, playerCode, externalRoundId,
                    request.ExternalGameId, request.GameType).ConfigureAwait(false);

            var gameActionExternalIds =
                await _services.GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId)
                    .ConfigureAwait(false);

            if (roundStatus == RoundStatus.Closed && gameActionExternalIds.Length == 0)
            {
                return _map.Response<CancelBetResponse>(request.PlayerCode, GameApiError.BetAlreadyCanceled);
            }

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r);
        }

        #endregion Private Methods
    }
}
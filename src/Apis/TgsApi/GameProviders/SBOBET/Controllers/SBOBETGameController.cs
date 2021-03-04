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
        private readonly ISBOBETGameControllerMapper _map;
        private readonly ISBOBETServices _services;
        private readonly ISettingOperations _settingOperations;

        public SBOBETGameController(
            IGamePlayActions actions,
            ISBOBETGameControllerMapper map,
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
        public async Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var r = await _actions.GetBalanceAsync(GameProviderConst.SBOBET, request.PlayerCode);
            return _map.Response(request.PlayerCode, r);
        }

        [HttpPost]
        [Route("Deduct")]
        public async Task<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                    .GameProviderConfigurations.SBOBET);

            if (configuration.DelaySecondsForDeduct > 0)
            {
                await Task.Delay(configuration.DelaySecondsForDeduct * 1000);
            }

            var data = await _services.GetGameProviderConfigurationDataAsync();

            var betData = _map.Request(data, request);
            var r = await _actions.PlaceBetAsync(betData);
            return _map.Response(request, r);
        }

        [HttpPost]
        [Route("Settle")]
        public async Task<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var actionTypes = new[] {WinBet, LoseBet, TieBet, FreeBet};

            var data = await _services.GetGameProviderConfigurationDataAsync();
            var gameProviderId = data.GameProviderId;
            var externalRoundId = request.ExternalTransactionId;

            var playerCode = request.PlayerCode;
            var gameId = await _services.GetGameIdAsync(gameProviderId, request.ExternalGameId, data.PlatformType);

            var roundStatus = await _services.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId);
            var gameActionExternalIds = await _services.GetGameActionExternalIdsAsync(gameProviderId,
                playerCode, externalRoundId, gameId, actionTypes);

            var betData = _map.Request(data, externalRoundId, request, roundStatus, gameActionExternalIds);
            var r = await _actions.SettleBetAsync(betData);
            return _map.Response(request, r);
        }

        /// <summary>
        ///     case: Cancel Bet or Void Round
        /// </summary>
        [HttpPost]
        [Route("Cancel")]
        public async Task<CancelBetResponse> CancelBetAsync(CancelBetRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();
            Func<GameProviderConfigurationData, CancelBetRequest, Task<CancelBetResponse>> func =
                request.IsCancelAll ? VoidRoundAsync : CancelBetAsync;

            return await func(data, request);
        }

        /// <summary>
        ///     case: Cancel Settle or Cancel CancelTransaction
        /// </summary>
        [HttpPost]
        [Route("Rollback")]
        public async Task<RollbackResponse> RollbackAsync(RollbackRequest request)
        {
            var actionTypes = new[] {WinBet, LoseBet, TieBet, FreeBet, CancelTransaction};

            var data = await _services.GetGameProviderConfigurationDataAsync();
            var externalRoundId = request.ExternalTransactionId;

            var gameProviderId = data.GameProviderId;
            var gameId = await _services.GetGameIdAsync(gameProviderId, request.ExternalGameId, data.PlatformType);

            var gameActionExternalIds =
                await _services.GetGameActionExternalIdsAsync(gameProviderId, request.PlayerCode, externalRoundId,
                    gameId, actionTypes);

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }

        [HttpPost]
        [Route("GetBetStatus")]
        public async Task<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();

            var gameProviderId = data.GameProviderId;
            var gameId = await _services.GetGameIdAsync(gameProviderId, request.ExternalGameId, data.PlatformType);

            var ga = await _services.GetValidGameActionsAsync(gameProviderId, request.PlayerCode,
                request.ExternalRoundId,
                gameId);

            if (ga == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalTransactionId} and transactionId: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            return _map.Response(request, ga);
        }

        #region Private Methods

        private async Task<CancelBetResponse> CancelBetAsync(GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var betData = _map.Request(data, request);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }

        private async Task<CancelBetResponse> VoidRoundAsync(GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var gameProviderId = data.GameProviderId;
            var externalRoundId = request.ExternalRoundId;

            var playerCode = request.PlayerCode;
            var gameId = await _services.GetGameIdAsync(data.GameProviderId, request.ExternalGameId, data.PlatformType);

            var roundStatus =
                await _services.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId);

            var gameActionExternalIds =
                await _services.GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId);

            if (roundStatus == RoundStatus.Closed && gameActionExternalIds.Length == 0)
            {
                return _map.Response<CancelBetResponse>(request.PlayerCode, GameApiError.BetAlreadyCanceled);
            }

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }

        #endregion Private Methods
    }
}
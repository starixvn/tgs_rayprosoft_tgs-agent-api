using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    public class BaseBetService : IBetService
    {
        protected BaseBetService(
            HttpContext httpContext,
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations,
            IRedisCacheManager redisCacheManager)
        {
            HttpContext = httpContext;
            GamePlayActions = actions;
            ActionModelMap = map;
            Services = services;
            SettingOperations = settingOperations;
            RedisCacheManager = redisCacheManager;
        }

        protected IGamePlayActions GamePlayActions { get; }
        protected HttpContext HttpContext { get; }
        protected ISBOBETActionModelMapper ActionModelMap { get; }
        protected ISBOBETServices Services { get; }
        protected ISettingOperations SettingOperations { get; }
        protected IRedisCacheManager RedisCacheManager { get; }

        public virtual async ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var r = await GamePlayActions
                .GetBalanceAsync(Consts.GameProvider.SBOBET, request.PlayerCode)
                .ConfigureAwait(false);

            return ActionModelMap.Response(request.PlayerCode, r);
        }

        public virtual async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var configuration =
                await SettingOperations
                    .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET)
                    .ConfigureAwait(false);

            if (configuration.DelaySecondsForPlaceBet > 0)
            {
                await Task.Delay(configuration.DelaySecondsForPlaceBet * 1000).ConfigureAwait(false);
            }

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var (platformType, _) = await Services
                .GetPlatformTypeAndGameWithLoginRecordsAsync(gameProviderId, request.ExternalGameId,
                    request.PlayerCode).ConfigureAwait(false);

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = ActionModelMap.Request(data, request);
            var r = await GamePlayActions.PlaceBetAsync(betData).ConfigureAwait(false);
            return ActionModelMap.Response(request, r);
        }

        public virtual async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;

            var (_, _, platformType) =
                await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                    gameProviderId,
                    playerCode,
                    externalRoundId,
                    externalRoundId).ConfigureAwait(false);

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            var betData = ActionModelMap.Request(data, externalRoundId, request, hasRollback);
            var r = await GamePlayActions.SettleBetAsync(betData).ConfigureAwait(false);

            await RedisCacheManager.ClearCacheAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, true).ConfigureAwait(false);

            return ActionModelMap.Response(request, r);
        }

        /// <summary>
        ///     case: Cancel Bet or Void Round
        /// </summary>
        public virtual async ValueTask<CancelBetResponse> CancelBetAsync(CancelBetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            Func<Guid, Guid, CancelBetRequest, ValueTask<CancelBetResponse>> func =
                request.IsCancelAll ? VoidRoundAsync : CancelBetAsync;

            return await func(gameProviderId, gameProviderConfigurationId, request).ConfigureAwait(false);
        }

        /// <summary>
        ///     case: Cancel Settle or Cancel CancelTransaction
        /// </summary>
        public virtual async ValueTask<RollbackResponse> RollbackAsync(RollbackRequest request)
        {
            var playerCode = request.PlayerCode;
            var externalRoundId = request.ExternalRoundId;

            var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            if (hasRollback)
            {
                return ActionModelMap.Response<RollbackResponse>(playerCode, GameApiError.BetAlreadyRollback);
            }

            var actionTypes = new[] {WinBet, LoseBet, TieBet, FreeBet, CancelTransaction};

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var (_, gameId, platformType) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                externalRoundId).ConfigureAwait(false);

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var gameActionExternalIds =
                await Services
                    .GetGameActionExternalIdsAsync(gameProviderId, request.PlayerCode, externalRoundId, gameId,
                        actionTypes).ConfigureAwait(false);

            var betData = ActionModelMap.Request(data, request, gameActionExternalIds);
            var r = await GamePlayActions.CancelTransactionAsync(betData).ConfigureAwait(false);

            if (r.Transactions.All(t => !t.IsDuplicate))
            {
                await RedisCacheManager.SafeCacheForeverAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                    externalRoundId,
                    true);
            }

            return ActionModelMap.Response(request, r);
        }

        public virtual async ValueTask<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;

            var (_, gameId, _) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                externalRoundId).ConfigureAwait(false);

            var ga = await Services
                .GetValidGameActionsAsync(gameProviderId, request.PlayerCode, request.ExternalRoundId, gameId)
                .ConfigureAwait(false);

            if (ga == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalTransactionId} and transactionId: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            return ActionModelMap.Response(request, ga);
        }

        private async ValueTask<CancelBetResponse> CancelBetAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, CancelBetRequest request)
        {
            var (_, _, platformType) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                request.PlayerCode,
                request.ExternalRoundId,
                request.ExternalTransactionId).ConfigureAwait(false);

            var data = await Services.GetGameProviderConfigurationDataAsync(gameProviderId,
                gameProviderConfigurationId, platformType);

            var betData = ActionModelMap.Request(data, request);
            var r = await GamePlayActions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return ActionModelMap.Response(request, r);
        }

        private async ValueTask<CancelBetResponse> VoidRoundAsync(
            Guid gameProviderId,
            Guid gameProviderConfigurationId,
            CancelBetRequest request)
        {
            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;

            var (roundStatus, gameId, platformType) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                request.ExternalTransactionId).ConfigureAwait(false);

            var gameActionExternalIds =
                await Services.GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId)
                    .ConfigureAwait(false);

            if (roundStatus == RoundStatus.Closed && gameActionExternalIds.Length == 0)
            {
                return ActionModelMap.Response<CancelBetResponse>(request.PlayerCode, GameApiError.BetAlreadyCanceled);
            }

            var data = await Services.GetGameProviderConfigurationDataAsync(gameProviderId,
                gameProviderConfigurationId, platformType);

            var betData = ActionModelMap.Request(data, request, gameActionExternalIds);
            var r = await GamePlayActions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return ActionModelMap.Response(request, r);
        }
    }
}
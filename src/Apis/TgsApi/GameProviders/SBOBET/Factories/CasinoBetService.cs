using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    internal class CasinoBetService : BaseBetService
    {
        public CasinoBetService(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations,
            IRedisCacheManager redisCacheManager)
            : base(
                httpContextAccessor.HttpContext,
                actions,
                map,
                services,
                settingOperations,
                redisCacheManager)
        {
        }

        public override async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var playerCode = request.PlayerCode;
            var externalRoundId = request.ExternalTransactionId;
            var externalGameId = request.ExternalGameId;

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var (platformType, games) = await Services
                .GetPlatformTypeAndGameWithLoginRecordsAsync(gameProviderId, externalGameId,
                    request.PlayerCode).ConfigureAwait(false);

            var gameId = games.Where(g => g.ExternalId.IgnoreCaseEquals(externalGameId))
                .Select(g => g.Id).FirstOrDefault();

            var gameActionData =
                await Services.GetValidGameActionsAsync(gameProviderId, playerCode, externalRoundId, gameId)
                    .ConfigureAwait(false);

            if (gameActionData == null)
            {
                var res = await base.PlaceBetAsync(request).ConfigureAwait(false);

                await CachePlaceBetTransaction(externalRoundId, request.Amount, false);

                return res;
            }

            if (gameActionData.Round.Status == RoundStatus.Closed)
            {
                var cancellationReferenceIds = gameActionData.GameActions
                    .Where(ga => ga.Type == GameActionType.CancelTransaction).Select(ga => ga.ReferencedGameActionId)
                    .ToArray();

                var isVoid = gameActionData.GameActions.Where(ga => ga.Type != GameActionType.CancelTransaction)
                    .All(ga => cancellationReferenceIds.Contains(ga.Id));

                var error = isVoid ? GameApiError.BetAlreadyCanceled : GameApiError.BetAlreadySettled;
                return ActionModelMap.Response<PlaceBetResponse>(playerCode, error);
            }

            var bet = await RedisCacheManager.SafeGetAsync(
                SBOBETConsts.RedisKey.BetTransaction,
                externalRoundId, () => Task.FromResult((BetTransaction)null)).ConfigureAwait(false);

            var previousBetAmount = bet.Amount;
            if (previousBetAmount >= request.Amount)
            {
                return ActionModelMap.Response<PlaceBetResponse>(playerCode,
                    GameApiError.InvalidAmount);
            }

            var configuration =
                await SettingOperations
                    .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET)
                    .ConfigureAwait(false);

            if (configuration.DelaySecondsForPlaceBet > 0)
            {
                await Task.Delay(configuration.DelaySecondsForPlaceBet * 1000).ConfigureAwait(false);
            }

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = ActionModelMap.CancelAndPlaceBetRequest(data, request);
            var r = await GamePlayActions.PlaceBetAsync(betData).ConfigureAwait(false);

            await CachePlaceBetTransaction(externalRoundId, request.Amount, true);

            return ActionModelMap.Response(request, r);
        }

        public override async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;

            var (roundStatus, gameId, platformType) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                externalRoundId).ConfigureAwait(false);

            var gameActionExternalIds =
                await Services.GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId);

            if (roundStatus == RoundStatus.Closed)
            {
                if (gameActionExternalIds.Length == 0)
                {
                    return ActionModelMap.Response<SettleBetResponse>(playerCode, GameApiError.BetAlreadyCanceled);
                }

                if (gameActionExternalIds.Any(ga => ga.GameActionType.IsSettleBet()))
                {
                    return ActionModelMap.Response<SettleBetResponse>(playerCode, GameApiError.BetAlreadySettled);
                }
            }

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            var betData = ActionModelMap.Request(data, externalRoundId, request, hasRollback, true);
            var r = await GamePlayActions.SettleBetAsync(betData).ConfigureAwait(false);

            await RedisCacheManager
                .ClearCacheByBatchAsync(new CacheKey[]
                {
                    new()
                    {
                        CacheType = SBOBETConsts.RedisKey.RollbackTransaction,
                        Key = externalRoundId
                    },
                    new()
                    {
                        CacheType = SBOBETConsts.RedisKey.BetTransaction,
                        Key = externalRoundId
                    }
                }, true).ConfigureAwait(false);

            return ActionModelMap.Response(request, r);
        }

        private async Task CachePlaceBetTransaction(string externalRoundId, decimal amount, bool isAdjust) =>
            await RedisCacheManager.SafeCacheForeverAsync(SBOBETConsts.RedisKey.BetTransaction,
                externalRoundId, new BetTransaction(amount, isAdjust)).ConfigureAwait(false);

        private record BetTransaction(decimal Amount, bool IsAdjust);
    }
}
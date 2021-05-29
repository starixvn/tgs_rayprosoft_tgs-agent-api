using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
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
    internal sealed class VirtualSportBetService : BaseBetService
    {
        public VirtualSportBetService(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions playActions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations,
            IRedisCacheManager redisCacheManager)
            : base(
                httpContextAccessor.HttpContext,
                playActions,
                map,
                services,
                settingOperations,
                redisCacheManager)
        {
        }

        public override async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
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

            var betData = ActionModelMap.Request(data, request, true);
            var r = await GamePlayActions.PlaceBetAsync(betData).ConfigureAwait(false);
            return ActionModelMap.Response(request, r);
        }

        public override async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var externalRoundId = request.ExternalTransactionId;
            var playerCode = request.PlayerCode;

            var (roundStatus, gameId, platformType) =
                await Services.GetRoundStatusAndGameIdAndPlatformAsync(gameProviderId, playerCode,
                    externalRoundId).ConfigureAwait(false);

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

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

            var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            var betData = ActionModelMap.Request(data, externalRoundId, request, hasRollback);
            var r = await GamePlayActions.SettleBetAsync(betData).ConfigureAwait(false);

            await RedisCacheManager.ClearCacheAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, true).ConfigureAwait(false);

            return ActionModelMap.Response(request, r);
        }
    }
}
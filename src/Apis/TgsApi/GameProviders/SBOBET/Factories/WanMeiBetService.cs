using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    public class WanMeiBetService : BaseBetService
    {
        private readonly IDatabase _redisDb;

        public WanMeiBetService(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations,
            IRedisCacheManager redisCacheManager,
            IRedisRepository redisRepository)
            : base(
                  httpContextAccessor.HttpContext,
                  actions,
                  map,
                  services,
                  settingOperations,
                  redisCacheManager)
        {
            if (redisRepository is null)
            {
                throw new ArgumentNullException(nameof(redisRepository));
            }

            _redisDb = redisRepository.GetDb();
        }

        public override async ValueTask<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;

            var (_, gameId, _) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                request.ExternalTransactionId).ConfigureAwait(false);

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

        public override async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var response = await base
                .PlaceBetAsync(request)
                .ConfigureAwait(false);

            await _redisDb
                .StringSetAsync(request.ExternalTransactionId, request.ExternalRoundId)
                .ConfigureAwait(false);

            return response;
        }

        public override async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var externalRoundId = await _redisDb
                .StringGetAsync(request.ExternalTransactionId)
                .ConfigureAwait(false);
            var playerCode = request.PlayerCode;

            var (roundStatus, _, platformType) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                request.ExternalTransactionId).ConfigureAwait(false);

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            if (roundStatus == RoundStatus.Closed)
                return new SettleBetResponse
                {
                    ErrorCode = 2002,
                    ErrorMessage = "Round already closed"
                };

            var betData = ActionModelMap.Request(data, externalRoundId, request, hasRollback);
            var r = await GamePlayActions.SettleBetAsync(betData).ConfigureAwait(false);

            await RedisCacheManager.ClearCacheAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, true).ConfigureAwait(false);

            return ActionModelMap.Response(request, r);
        }
    }
}

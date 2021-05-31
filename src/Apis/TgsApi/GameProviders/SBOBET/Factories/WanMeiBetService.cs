using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    public class WanMeiBetService : BaseBetService
    {
        private readonly IDatabase _redisDb;

        public WanMeiBetService(
            HttpContext httpContext,
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations,
            IRedisCacheManager redisCacheManager,
            IRedisRepository redisRepository)
            : base(
                  httpContext,
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
            var externalRoundId = request.ExternalTransactionId;
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
    }
}

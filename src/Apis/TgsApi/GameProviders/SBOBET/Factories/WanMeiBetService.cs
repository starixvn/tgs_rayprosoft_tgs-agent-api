using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.Extensions;
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
        public WanMeiBetService(
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

        public override async ValueTask<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var externalRoundId = request.ExternalRoundId;
            var externalTransactionId = request.ExternalTransactionId;
            var playerCode = request.PlayerCode;

            var (_, gameId, _) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                externalTransactionId).ConfigureAwait(false);

            var gameActionData = await Services
                .GetValidGameActionsAsync(gameProviderId, request.PlayerCode, request.ExternalRoundId, gameId)
                .ConfigureAwait(false);

            if (gameActionData == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalTransactionId} " +
                    $"and transactionId: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var ga = gameActionData.GameActions.FirstOrDefault(act =>
                act.ExternalTransactionId.IgnoreCaseEquals(externalTransactionId));

            if (ga == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalTransactionId} " +
                    $"and transactionId: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var isTransactionCancelled = gameActionData.GameActions.Any(act =>
                act.Type == GameActionType.CancelTransaction && act.ReferencedGameActionId == ga.Id);

            return ActionModelMap.WanMeiResponse(request, gameActionData, isTransactionCancelled);
        }

        public override async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var response = await base.PlaceBetAsync(request).ConfigureAwait(false);

            var transactions = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.BetTransaction,
                    request.ExternalRoundId, () => Task.FromResult(new Dictionary<string, BetTransaction>()))
                .ConfigureAwait(false);

            var transactionId = request.ExternalTransactionId;
            if (!transactions.ContainsKey(transactionId))
            {
                transactions[transactionId] = new(transactionId);
            }

            await RedisCacheManager.SafeCacheForeverAsync(SBOBETConsts.RedisKey.BetTransaction,
                request.ExternalRoundId, transactions).ConfigureAwait(false);

            return response;
        }

        public override async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var transactions = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.BetTransaction,
                    request.ExternalRoundId, () => Task.FromResult(new Dictionary<string, BetTransaction>()))
                .ConfigureAwait(false);

            var externalRoundId = request.ExternalRoundId;
            var transaction = transactions.Values.FirstOrDefault(t => !t.IsSettled);

            if (transaction == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var externalTransactionId = transaction.ExternalTransactionId;
            var playerCode = request.PlayerCode;

            var (roundStatus, _, platformType) = await Services.GetRoundStatusAndGameIdAndPlatformAsync(
                gameProviderId,
                playerCode,
                externalRoundId,
                externalTransactionId).ConfigureAwait(false);

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            //var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
            //    externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            //if (roundStatus == RoundStatus.Closed)
            //    return ActionModelMap.Response<SettleBetResponse>(playerCode, GameApiError.BetAlreadyCanceled);

            var betData = ActionModelMap.WanMeiRequest(data, externalTransactionId, externalRoundId, request);
            var r = await GamePlayActions.SettleBetAsync(betData).ConfigureAwait(false);

            transactions[externalTransactionId] = transaction with {IsSettled = true};
            await RedisCacheManager.SafeCacheForeverAsync(SBOBETConsts.RedisKey.BetTransaction,
                request.ExternalRoundId, transactions).ConfigureAwait(false);

            //await RedisCacheManager.ClearCacheAsync(SBOBETConsts.RedisKey.RollbackTransaction,
            //    externalRoundId, true).ConfigureAwait(false);

            return ActionModelMap.Response(request, r);
        }


        private record BetTransaction(string ExternalTransactionId, bool IsSettled = false, bool IsCancelled = false);
    }
}
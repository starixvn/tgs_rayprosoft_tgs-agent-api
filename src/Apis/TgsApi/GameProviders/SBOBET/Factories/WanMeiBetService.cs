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
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    public class WanMeiBetService : BaseBetService
    {
        private readonly IGameProviderOperations _gameProviderOperations;

        public WanMeiBetService(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations,
            IRedisCacheManager redisCacheManager,
            IGameProviderOperations gameProviderOperations)
            : base(
                httpContextAccessor.HttpContext,
                actions,
                map,
                services,
                settingOperations,
                redisCacheManager)
        {
            _gameProviderOperations = gameProviderOperations;
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
                .GetGameActionDataByRoundAsync(gameProviderId, request.PlayerCode, request.ExternalRoundId, gameId)
                .ConfigureAwait(false);

            if (gameActionData == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalTransactionId} " +
                    $"and transactionId: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            return ActionModelMap.WanMeiResponse(request, gameActionData);
        }

        public override async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, request.ExternalGameId)
                .ConfigureAwait(false);
            var game = games.FirstOrDefault();

            if (game == null)
            {
                throw new InvalidGameActionException($"Game '{request.ExternalGameId}' is not found",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            var gameActionData =
                await Services.GetGameActionDataByRoundAsync(gameProviderId, playerCode, externalRoundId, game.Id)
                    .ConfigureAwait(false);

            if (gameActionData == null)
            {
                throw new InvalidGameActionException(
                    $"Transactions of transferCode: {externalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var gameActions = gameActionData.GameActions;
            if (gameActionData.Round.Status == RoundStatus.Closed)
            {
                var error = gameActions.Any(ga => ga.Type.IsSettleBet())
                    ? GameApiError.BetAlreadySettled
                    : GameApiError.BetAlreadyCanceled;

                return ActionModelMap.Response<SettleBetResponse>(playerCode, error);
            }

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId,
                    PlatformType.Desktop)
                .ConfigureAwait(false);

            var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            string externalTransactionId = null;

            if (hasRollback)
            {
                externalTransactionId = gameActions.Where(ga => ga.Type == PlaceBet)
                    .Select(ga => ga.ExternalTransactionId).FirstOrDefault();
            }
            else
            {
                externalTransactionId = gameActions.Where(ga => ga.Type == PlaceBet
                        && !gameActions.Any(a => a.ReferencedGameActionId == ga.Id &&
                            a.Type == CancelTransaction))
                    .Select(ga => ga.ExternalTransactionId).FirstOrDefault();
            }

            var betData =
                ActionModelMap.WanMeiRequest(data, externalTransactionId, externalRoundId, request, hasRollback);
            var r = await GamePlayActions.SettleBetAsync(betData).ConfigureAwait(false);

            // WanMei settle will force close round
            await RedisCacheManager.ClearCacheAsync(SBOBETConsts.RedisKey.RollbackTransaction, externalRoundId, true)
                .ConfigureAwait(false);

            return ActionModelMap.Response(request, r);
        }

        /// <summary>
        ///     case: Cancel Bet or Void Round
        /// </summary>
        public override async ValueTask<CancelBetResponse> CancelBetAsync(CancelBetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            Func<Guid, Guid, CancelBetRequest, ValueTask<CancelBetResponse>> func =
                request.IsCancelAll ? VoidRoundAsync : CancelBetAsync;

            return await func(gameProviderId, gameProviderConfigurationId, request).ConfigureAwait(false);
        }


        public override async ValueTask<RollbackResponse> RollbackAsync(RollbackRequest request)
        {
            var playerCode = request.PlayerCode;
            var externalRoundId = request.ExternalRoundId;

            var hasRollback = await RedisCacheManager.SafeGetAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                externalRoundId, () => Task.FromResult(false)).ConfigureAwait(false);

            if (hasRollback)
            {
                return ActionModelMap.Response<RollbackResponse>(playerCode, GameApiError.BetAlreadyRollback);
            }

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, request.ExternalGameId)
                .ConfigureAwait(false);
            var game = games.FirstOrDefault();

            if (game == null)
            {
                throw new InvalidGameActionException($"Game '{request.ExternalGameId}' is not found",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            var gameActionData =
                await Services.GetGameActionDataByRoundAsync(gameProviderId, playerCode, externalRoundId, game.Id)
                    .ConfigureAwait(false);

            if (gameActionData == null)
            {
                throw new InvalidGameActionException(
                    $"Transactions of transferCode: {externalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var data = await Services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId,
                    PlatformType.Desktop)
                .ConfigureAwait(false);

            var betData = ActionModelMap.WanMeiRequest(data, request, gameActionData);
            var r = await GamePlayActions.CancelTransactionAsync(betData).ConfigureAwait(false);

            if (r.Transactions.All(t => !t.IsDuplicate))
            {
                await RedisCacheManager.SafeCacheForeverAsync(SBOBETConsts.RedisKey.RollbackTransaction,
                    externalRoundId, true);
            }

            return ActionModelMap.Response(request, r);
        }

        private async ValueTask<CancelBetResponse> CancelBetAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, CancelBetRequest request)
        {
            var playerCode = request.PlayerCode;
            var externalRoundId = request.ExternalRoundId;
            var externalTransactionId = request.ExternalTransactionId;

            var (_, gameId, platformType) = await Services
                .GetRoundStatusAndGameIdAndPlatformAsync(
                    gameProviderId,
                    playerCode,
                    externalRoundId,
                    externalTransactionId).ConfigureAwait(false);

            var gameActionData =
                await Services.GetGameActionDataByRoundAsync(gameProviderId, playerCode, externalRoundId, gameId)
                    .ConfigureAwait(false);

            if (gameActionData == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {externalRoundId} " +
                    $"and transactionId: {externalTransactionId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var ga = gameActionData.GameActions.FirstOrDefault(act =>
                act.ExternalTransactionId.IgnoreCaseEquals(externalTransactionId));

            if (ga == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {externalRoundId} " +
                    $"and transactionId: {externalTransactionId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var isCancelled = gameActionData.GameActions.Any(act =>
                act.Type == CancelTransaction && act.ReferencedGameActionId == ga.Id &&
                !gameActionData.GameActions.Any(refe => refe.Type == CancelTransaction &&
                    refe.ReferencedGameActionId == act.Id));

            if (isCancelled)
            {
                return ActionModelMap.Response<CancelBetResponse>(playerCode, GameApiError.BetAlreadyCanceled);
            }

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Saba.Attributes;
using TGS.Apis.TgsApi.GameProviders.Saba.Data;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Controllers
{
    [Route("api/saba")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Saba)]
    public class SabaGameController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Saba;

        private readonly IGamePlayActions _actions;
        private readonly IBrandOperations _brandOperations;
        private readonly IRedisCacheManager _cacheManager;

        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISabaActionModelMapper _map;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISabaServices _services;

        public SabaGameController(
            IGamePlayActions actions,
            ISabaActionModelMapper map,
            ISabaServices services,
            IRedisCacheManager cacheManager,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations)
        {
            _actions = actions;
            _map = map;
            _services = services;
            _cacheManager = cacheManager;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
        }

        [HttpPost]
        [Route("getbalance")]
        [ResolveRequest(Type = typeof(GetBalanceRequest))]
        public async ValueTask<GetBalanceResponse> GetPlayerBalanceAsync(GetBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _actions.GetBalanceAsync(GameProviderCode, playerCode)
                .ConfigureAwait(false);

            return _map.Response(request.Body.UserId, r);
        }

        [HttpPost]
        [Route("placebet")]
        [ResolveRequest(Type = typeof(PlaceBetRequest))]
        public async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.Request(playerCode, data, request);
            var r = await _actions.LockAmountAsync(betData).ConfigureAwait(false);

            await CacheLockAmountAsync(betData.Transactions);

            return _map.Response(request.Body, r);
        }

        [HttpPost]
        [Route("placebetparlay")]
        [ResolveRequest(Type = typeof(PlaceBetParlayRequest))]
        public async ValueTask<PlaceBetParlayResponse> PlaceBetParlayAsync(PlaceBetParlayRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.Request(playerCode, data, request);
            var r = await _actions.LockAmountAsync(betData).ConfigureAwait(false);

            await CacheLockAmountAsync(betData.Transactions);

            return _map.Response(request.Body, r);
        }

        [HttpPost]
        [Route("confirmbet")]
        [ResolveRequest(Type = typeof(ConfirmBetRequest))]
        public async ValueTask<ConfirmBetResponse> ConfirmBetAsync(ConfirmBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var confirmBet = request.Body.Transactions[0];
            var externalRoundId = confirmBet.ExternalRoundId;
            var unlockAmount = confirmBet.IsOddsChanged
                ? await GetLockAmountAsync(playerCode, data.GameProviderId, externalRoundId).ConfigureAwait(false)
                : confirmBet.ActualAmount;

            var betData = _map.Request(playerCode, data, request, unlockAmount);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            var transactions = betData.Transactions;
            await RemoveLockAmountCacheAsync(transactions).ConfigureAwait(false);
            await CacheExternalRoundIdMapAsync(transactions).ConfigureAwait(false);

            return _map.Response(request.Body, r);
        }

        [HttpPost]
        [Route("confirmbetparlay")]
        [ResolveRequest(Type = typeof(ConfirmBetParlayRequest))]
        public async ValueTask<ConfirmBetParlayResponse> ConfirmBetParlayAsync(ConfirmBetParlayRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var unlockAmounts = new Dictionary<string, decimal>();

            foreach (var confirmBet in request.Body.Transactions)
            {
                var externalRoundId = confirmBet.ExternalRoundId;
                var unlockAmount = confirmBet.IsOddsChanged
                    ? await GetLockAmountAsync(playerCode, data.GameProviderId, externalRoundId).ConfigureAwait(false)
                    : confirmBet.ActualAmount;

                unlockAmounts[externalRoundId] = unlockAmount;
            }

            var betData = _map.Request(playerCode, data, request, unlockAmounts);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            var transactions = betData.Transactions;
            await RemoveLockAmountCacheAsync(transactions).ConfigureAwait(false);
            await CacheExternalRoundIdMapAsync(transactions).ConfigureAwait(false);

            return _map.Response(request.Body, r);
        }

        /// <summary>
        ///     This action is to cancel place bet / place bet parlay
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("cancelbet")]
        [ResolveRequest(Type = typeof(CancelBetRequest))]
        public async ValueTask<CancelBetResponse> CancelBetAsync(CancelBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.Request(playerCode, data, request);
            var r = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);

            await RemoveLockAmountCacheAsync(betData.Transactions).ConfigureAwait(false);

            return _map.Response(request.Body, r);
        }

        [Route("settle")]
        [ResolveRequest(Type = typeof(SettleBetRequest))]
        public async ValueTask<SettleBetResponse> SettleAsync(SettleBetRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var autoCloseRound =
                await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);

            var isTestEnv = HttpContext.Items.Get(SabaConsts.ContextItems.IsTestEnvironment, false);

            var transactions = request.Body.Transactions.Select(t =>
            {
                var playerCode = _services.GetPlayerCode(t.UserId, isTestEnv);

                return new SettleTransaction(
                    playerCode, t.ExternalTransactionId.ToString(),
                    t.UpdateTime, t.SettleDate, SabaConsts.GameActionTypeMap[t.Status],
                    t.Payout);
            }).ToArray();

            var playerConfigurationIds = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var externalRoundIds = await GetExternalRoundIdMapCacheAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var grouping = (from t in transactions
                            let type = t.GameActionType switch
                            {
                                GameActionType.CancelTransaction => "cancel",
                                _                                => "settle"
                            }
                            select new
                            {
                                Type = type,
                                Data = t
                            }).ToLookup(v => v.Type, v => v.Data);

            var cancelTransactions = grouping.Contains("cancel")
                ? grouping["cancel"].ToArray()
                : Array.Empty<SettleTransaction>();

            var settleTransactions = grouping.Contains("settle")
                ? grouping["settle"].ToArray()
                : Array.Empty<SettleTransaction>();

            var results = await ExecuteSettleAndVoidRoundAsync(gameProviderId, playerConfigurationIds,
                externalRoundIds, autoCloseRound, settleTransactions, cancelTransactions).ConfigureAwait(false);

            await RemoveExternalRoundIdMapCacheAsync(transactions).ConfigureAwait(false);

            return _map.Response(request, results);
        }

        [Route("unsettle")]
        [ResolveRequest(Type = typeof(UnsettleBetRequest))]
        public async ValueTask<UnsettleBetResponse> UnsettleAsync(UnsettleBetRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var autoCloseRound =
                await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);

            var isTestEnv = HttpContext.Items.Get(SabaConsts.ContextItems.IsTestEnvironment, false);

            var transactions = request.Body.Transactions.Select(t =>
            {
                var playerCode = _services.GetPlayerCode(t.UserId, isTestEnv);

                return new TransactionInfo(
                    playerCode, t.ExternalTransactionId.ToString(), t.UpdateTime);
            }).ToArray();

            var playerConfigurationIds = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var externalRoundIds = await GetExternalRoundIdMapCacheAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var batchBetData = _map.UnsettleBatchRequest(gameProviderId, autoCloseRound, playerConfigurationIds,
                externalRoundIds, transactions);

            var result = await _actions.CancelBatchAsync(batchBetData);

            return _map.Response(request, result);
        }

        [Route("resettle")]
        [ResolveRequest(Type = typeof(ResettleBetRequest))]
        public async ValueTask<ResettleBetResponse> ResettleBetAsync(ResettleBetRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var autoCloseRound =
                await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);

            var isTestEnv = HttpContext.Items.Get(SabaConsts.ContextItems.IsTestEnvironment, false);

            var transactions = request.Body.Transactions.Select(t =>
            {
                var playerCode = _services.GetPlayerCode(t.UserId, isTestEnv);

                return new SettleTransaction(
                    playerCode, t.ExternalTransactionId.ToString(),
                    t.UpdateTime, t.SettleDate, SabaConsts.GameActionTypeMap[t.Status],
                    t.Payout);
            }).ToArray();

            var playerConfigurationIds = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var externalRoundIds = await GetExternalRoundIdMapCacheAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var grouping = (from t in transactions
                            let type = t.GameActionType switch
                            {
                                GameActionType.CancelTransaction => "cancel",
                                _                                => "settle"
                            }
                            select new
                            {
                                Type = type,
                                Data = t
                            }).ToLookup(v => v.Type, v => v.Data);

            var cancelTransactions = grouping.Contains("cancel")
                ? grouping["cancel"].ToArray()
                : Array.Empty<SettleTransaction>();

            var settleTransactions = grouping.Contains("settle")
                ? grouping["settle"].ToArray()
                : Array.Empty<SettleTransaction>();

            var results = await ExecuteSettleAndVoidRoundAsync(gameProviderId, playerConfigurationIds,
                externalRoundIds, autoCloseRound, settleTransactions, cancelTransactions, true).ConfigureAwait(false);

            await RemoveExternalRoundIdMapCacheAsync(transactions).ConfigureAwait(false);

            return _map.Response(request, results);
        }

        #region Private Methods

        private async ValueTask CacheLockAmountAsync(TransactionData[] transactions)
        {
            foreach (var t in transactions)
            {
                await _cacheManager.SafeCacheAsync(SabaConsts.RedisKey.LockAmount, t.ExternalRoundId,
                    TimeSpan.FromHours(12), t.Amount);
            }
        }

        private async ValueTask CacheExternalRoundIdMapAsync(TransactionData[] transactions)
        {
            foreach (var t in transactions.Where(t => t.GameActionType == GameActionType.PlaceBet))
            {
                await _cacheManager.SafeCacheForeverAsync(SabaConsts.RedisKey.SabaTxIdMap, t.ExternalTransactionId,
                    t.ExternalRoundId);
            }
        }

        private async ValueTask<decimal> GetLockAmountAsync(string playerCode, Guid gameProviderId,
            string externalRoundId) =>
            await _cacheManager.SafeGetAsync(SabaConsts.RedisKey.LockAmount, externalRoundId,
                async () => await _services.GetLockAmountAsync(playerCode, gameProviderId, externalRoundId)
                    .ConfigureAwait(false));

        private async ValueTask RemoveLockAmountCacheAsync(TransactionData[] transactions)
        {
            var cacheKeys = transactions.Select(t => new CacheKey
            {
                CacheType = SabaConsts.RedisKey.LockAmount,
                Key = t.ExternalRoundId
            }).ToArray();

            await _cacheManager.ClearCacheByBatchAsync(cacheKeys, true).ConfigureAwait(false);
        }

        private async ValueTask<Dictionary<string, string>> GetExternalRoundIdMapCacheAsync(Guid gameProviderId,
            IEnumerable<TransactionInfo> transactions)
        {
            var map = new Dictionary<string, string>();

            foreach (var t in transactions)
            {
                var externalTransactionId = t.ExternalTransactionId;
                var externalRoundId =
                    await _cacheManager.SafeGetAsync(SabaConsts.RedisKey.SabaTxIdMap, externalTransactionId,
                        async () => await _services
                            .GetExternalRoundIdAsync(gameProviderId, t.PlayerCode, externalTransactionId)
                            .ConfigureAwait(false));

                map[externalTransactionId] = externalRoundId;
            }

            return map;
        }

        private async ValueTask RemoveExternalRoundIdMapCacheAsync(IEnumerable<TransactionInfo> transactions)
        {
            var cacheKeys = transactions.Select(t => new CacheKey
            {
                CacheType = SabaConsts.RedisKey.SabaTxIdMap,
                Key = t.ExternalTransactionId
            }).ToArray();

            await _cacheManager.ClearCacheByBatchAsync(cacheKeys, true).ConfigureAwait(false);
        }

        private async ValueTask<Dictionary<string, Guid>> GetPlayerGameProviderConfigurationIdsAsync(
            Guid gameProviderId, IEnumerable<TransactionInfo> transactions)
        {
            var map = new Dictionary<string, Guid>();

            foreach (var playerCode in transactions.GroupBy(t => t.PlayerCode).Select(g => g.Key))
            {
                if (map.ContainsKey(playerCode))
                {
                    continue;
                }

                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode)
                    .ConfigureAwait(false);

                var gameProviderConfigurationId =
                    await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId)
                        .ConfigureAwait(false);

                map.Add(playerCode, gameProviderConfigurationId);
            }

            return map;
        }


        private async ValueTask<BatchBetResult[]> ExecuteSettleAndVoidRoundAsync(Guid gameProviderId,
            Dictionary<string, Guid> playerConfigurationIds, Dictionary<string, string> externalRoundIds,
            bool autoCloseRound, SettleTransaction[] settleTransactions, SettleTransaction[] cancelTransactions,
            bool isResettle = false)
        {
            var betResults = new List<BatchBetResult>();

            if (cancelTransactions.Length > 0)
            {
                var cancelResult = await VoidRoundAsync(gameProviderId,
                    autoCloseRound, playerConfigurationIds, externalRoundIds, cancelTransactions).ConfigureAwait(false);

                if (cancelResult != null)
                {
                    betResults.Add(cancelResult);
                }
            }

            if (settleTransactions.Length == 0)
            {
                return betResults.ToArray();
            }

            var settleResult = await SettleBatchAsync(gameProviderId, autoCloseRound, playerConfigurationIds,
                externalRoundIds, settleTransactions, isResettle).ConfigureAwait(false);

            if (settleResult != null)
            {
                betResults.Add(settleResult);
            }

            return betResults.ToArray();
        }

        private ValueTask<BatchBetResult> SettleBatchAsync(Guid gameProviderId, bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds, Dictionary<string, string> externalRoundIds,
            SettleTransaction[] transactions, bool isResettle)
        {
            var batchBetData = isResettle
                ? _map.ResettleBatchRequest(gameProviderId, autoCloseRound, playerConfigurationIds,
                    externalRoundIds, transactions)
                : _map.SettleBatchRequest(gameProviderId, autoCloseRound, playerConfigurationIds,
                    externalRoundIds, transactions);

            return _actions.SettleBatchAsync(batchBetData);
        }

        private async ValueTask<BatchBetResult> VoidRoundAsync(Guid gameProviderId,
            bool autoCloseRound, Dictionary<string, Guid> playerConfigurationIds,
            Dictionary<string, string> externalRoundIds, SettleTransaction[] transactions)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, SabaConsts.ExternalGameId)
                .ConfigureAwait(false);

            if (games == null || games.Length == 0)
            {
                throw new InvalidGameActionException("No game is found", InvalidGameActionErrorCode.GameNotFound);
            }

            var gameId = games[0].Id;
            var playerGameActionExternalIds = new Dictionary<string, List<GameActionExternalIds>>();

            foreach (var g in transactions.GroupBy(t => t.PlayerCode))
            {
                var playerCode = g.Key;
                var gameActionExternalIds = new List<GameActionExternalIds>();

                foreach (var t in g.ToArray())
                {
                    var externalTransactionId = t.ExternalTransactionId;
                    var externalRoundId = externalRoundIds[externalTransactionId];

                    var externalIds =
                        await _services
                            .GetGameActionExternalIdsByRoundIdAsync(gameProviderId, t.PlayerCode, externalRoundId,
                                gameId)
                            .ConfigureAwait(false);

                    gameActionExternalIds.AddRange(externalIds);
                }

                playerGameActionExternalIds[playerCode] = gameActionExternalIds;
            }

            if (playerGameActionExternalIds.Count == 0)
            {
                return null;
            }

            var batchBetData =
                _map.VoidRoundRequest(gameProviderId, autoCloseRound, playerConfigurationIds,
                    playerGameActionExternalIds);

            return await _actions.CancelBatchAsync(batchBetData).ConfigureAwait(false);
        }

        #endregion
    }
}
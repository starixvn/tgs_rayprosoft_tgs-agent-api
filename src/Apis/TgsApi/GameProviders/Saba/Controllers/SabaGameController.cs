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
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Core.Infrastructures.Extensions;
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
            var playerCode = request.Body.PlayerCode;
            var r = await _actions.GetBalanceAsync(GameProviderCode, request.Body.PlayerCode)
                .ConfigureAwait(false);

            return _map.Response(playerCode, r);
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

            return _map.Response(playerCode, request.Body, r);
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

            return _map.Response(playerCode, request.Body, r);
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
            var externalRoundId = confirmBet.ExternalRoundId.ToString();
            var unlockAmount = confirmBet.IsOddsChanged
                ? await GetLockAmountAsync(playerCode, data.GameProviderId, externalRoundId).ConfigureAwait(false)
                : confirmBet.ActualAmount;

            var betData = _map.Request(playerCode, data, request, unlockAmount);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            var transactions = betData.Transactions;
            await RemoveLockAmountCacheAsync(transactions).ConfigureAwait(false);
            await CacheExternalRoundIdMapAsync(transactions).ConfigureAwait(false);

            return _map.Response(playerCode, request.Body, r);
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
                var externalRoundId = confirmBet.ExternalRoundId.ToString();
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

            return _map.Response(playerCode, request.Body, r);
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

            return _map.Response(playerCode, request.Body, r);
        }

        [Route("settle")]
        [ResolveRequest(Type = typeof(SettleBetRequest))]
        public async ValueTask<SettleBetResponse> SettleAsync(SettleBetRequest request)
        {
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var autoCloseRound =
                await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);

            var transactions = request.Body.Transactions;
            var playerConfigurationIds = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var externalRoundIds = await GetExternalRoundIdMapCacheAsync(gameProviderId, transactions)
                .ConfigureAwait(false);

            var grouping = (from t in transactions
                            group t by t.PlayerCode
                            into g
                            select g).ToDictionary(g => g.Key, g => g.ToArray());

            var responses = await grouping.Select(async kvp =>
            {
                var playerCode = kvp.Key;
                var gameProviderConfigurationId = playerConfigurationIds[playerCode];

                var data = new GameProviderConfigurationData
                {
                    GameProviderId = gameProviderId,
                    GameProviderConfigurationId = gameProviderConfigurationId,
                    AutoCloseRound = autoCloseRound,
                    PlatformType = PlatformType.Desktop
                };

                var cancelTransactions = kvp.Value
                    .Where(t => SabaConsts.GameActionTypeMap[t.Status] == GameActionType.CancelTransaction)
                    .Select(t => t).ToArray();

                var settleTransactions = kvp.Value.Except(cancelTransactions).ToArray();

                var betResults = new List<BatchBetResult>();

                if (cancelTransactions.Length > 0)
                {
                    var r = await VoidRoundAsync(data, playerCode, externalRoundIds, cancelTransactions)
                        .ConfigureAwait(false);

                    if (r != null)
                    {
                        betResults.Add(r);
                    }
                }

                if (settleTransactions.Length > 0)
                {
                    var r = await SettleBatchAsync(data, playerCode, externalRoundIds, settleTransactions)
                        .ConfigureAwait(false);

                    if (r != null)
                    {
                        betResults.Add(r);
                    }
                }

                return new {Response = betResults.ToArray()};
            }).WhenAll();

            await RemoveExternalRoundIdMapCacheAsync(transactions).ConfigureAwait(false);

            var results = responses.SelectMany(r => r.Response).ToArray();

            return _map.Response(request, results);
        }

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
            foreach (var t in transactions)
            {
                await _cacheManager.SafeCacheForeverAsync(SabaConsts.RedisKey.SabaTxIdMap, t.ExternalTransactionId,
                    t.ExternalRoundId);
            }
        }

        private async ValueTask<decimal> GetLockAmountAsync(string playerCode, Guid gameProviderId,
            string externalRoundId) =>
            await _cacheManager.SafeGetAsync(SabaConsts.RedisKey.LockAmount, externalRoundId,
                async () => await _services.GetLockAmountAsync(playerCode, gameProviderId, externalRoundId));

        private async ValueTask RemoveLockAmountCacheAsync(TransactionData[] transactions)
        {
            var cacheKeys = transactions.Select(t => new CacheKey
            {
                CacheType = SabaConsts.RedisKey.LockAmount,
                Key = t.ExternalRoundId
            }).ToArray();

            await _cacheManager.ClearCacheByBatchAsync(cacheKeys, true);
        }

        private async ValueTask<Dictionary<string, string>> GetExternalRoundIdMapCacheAsync(Guid gameProviderId,
            SettleBetRequest.Transaction[] transactions)
        {
            var map = new Dictionary<string, string>();

            foreach (var t in transactions)
            {
                var externalTransactionId = t.ExternalTransactionId.ToString();
                var externalRoundId =
                    await _cacheManager.SafeGetAsync(SabaConsts.RedisKey.SabaTxIdMap, externalTransactionId,
                        async () => await _services.GetExternalRoundIdAsync(gameProviderId, t.PlayerCode,
                            externalTransactionId));

                map[externalTransactionId] = externalRoundId;
            }

            return map;
        }

        private async ValueTask RemoveExternalRoundIdMapCacheAsync(SettleBetRequest.Transaction[] transactions)
        {
            var cacheKeys = transactions.Select(t => new CacheKey
            {
                CacheType = SabaConsts.RedisKey.SabaTxIdMap,
                Key = t.ExternalTransactionId.ToString()
            }).ToArray();

            await _cacheManager.ClearCacheByBatchAsync(cacheKeys, true);
        }

        private async ValueTask<Dictionary<string, Guid>> GetPlayerGameProviderConfigurationIdsAsync(
            Guid gameProviderId, SettleBetRequest.Transaction[] transactions)
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

        private ValueTask<BatchBetResult> SettleBatchAsync(GameProviderConfigurationData data,
            string playerCode, Dictionary<string, string> externalRoundIds, SettleBetRequest.Transaction[] transactions)
        {
            var batchBetData =
                _map.SettleBatchRequest(data, playerCode, externalRoundIds, transactions);

            return _actions.SettleBatchAsync(batchBetData);
        }

        private async ValueTask<BatchBetResult> VoidRoundAsync(GameProviderConfigurationData data,
            string playerCode, Dictionary<string, string> externalRoundIds, SettleBetRequest.Transaction[] transactions)
        {
            var gameProviderId = data.GameProviderId;
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, SabaConsts.ExternalGameId);
            if (games == null || games.Length == 0)
            {
                throw new InvalidGameActionException("No game is found", InvalidGameActionErrorCode.GameNotFound);
            }

            var gameId = games[0].Id;
            var gameActionExternalIds = new List<GameActionExternalIds>();

            foreach (var t in transactions)
            {
                var externalTransactionId = t.ExternalTransactionId.ToString();
                var externalRoundId = externalRoundIds[externalTransactionId];

                var externalIds =
                    await _services.GetGameActionExternalIdsByRoundIdAsync(gameProviderId, playerCode, externalRoundId,
                        gameId);

                gameActionExternalIds.AddRange(externalIds);
            }

            if (gameActionExternalIds.Count == 0)
            {
                return null;
            }

            var batchBetData =
                _map.CancelBatchRequest(data, playerCode, gameActionExternalIds);

            return await _actions.CancelBatchAsync(batchBetData);
        }
    }
}
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.IMESports.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.IMESports.Common;
using TGS.Shared.Game.Models.IMESports.Enums;
using TGS.Shared.Game.Models.IMESports.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Controllers
{
    [Route("api/imesports")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(GameProviderConst.IMESports)]
    public sealed class IMESportsGameController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly IBrandOperations _brandOperations;
        private readonly IRedisCacheManager _cacheManager;
        private readonly IIMESportsActionModelMapper _map;
        private readonly IPlayerOperations _playerOperations;
        private readonly IIMESportsServices _services;
        private readonly ISettingOperations _settingOperations;

        public IMESportsGameController(
            IGamePlayActions actions,
            IIMESportsActionModelMapper map,
            IIMESportsServices services,
            IBrandOperations brandOperations,
            IRedisCacheManager cacheManager,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations)
        {
            _actions = actions;
            _map = map;
            _services = services;
            _brandOperations = brandOperations;
            _cacheManager = cacheManager;
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
        }

        [HttpPost]
        [Route("getbalance")]
        public async ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var playerCode = await _services.GetPlayerCodeAsync(request.PlayerCode);
            var r = await _actions.GetBalanceAsync(GameProviderConst.IMESports, playerCode)
                .ConfigureAwait(false);

            return _map.Response(request.PlayerCode, r);
        }

        [HttpPost]
        [Route("placebet")]
        public async ValueTask<BetResponse> PlaceBetAsync(BetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

            var transaction = request.Transactions.First();
            var playerCode = await _services.GetPlayerCodeAsync(transaction.PlayerCode);

            var redisKey = _services.GetRedisCacheKey(playerCode, transaction.ExternalGameId);
            var sessionToken = await ValidateSessionTokenAsync(redisKey, request.SessionToken);

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            var gameProviderConfigurationId = bgpc.GameProviderConfigurationId;
            var platformType = transaction.Platform.IgnoreCaseEquals(PlatformType.Mobile.ToString())
                ? PlatformType.Mobile
                : PlatformType.Desktop;

            var data = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = _map.Request(data, request, playerCode, true);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            await _cacheManager.SetKeyExpirationInAsync(IMESportsConsts.RedisKey.SessionToken,
                redisKey, TimeSpan.FromSeconds(sessionToken.TimeToLeave));

            return _map.Response(r);
        }

        [HttpPost]
        [Route("settle")]
        public async ValueTask<BetResponse> BatchAsync(BetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

            var config =
                await _settingOperations.GetValueFromJsonByKeyAsync<IMESportsConfiguration>(SettingConsts
                    .GameProviderConfigurations.IMESports);
            var data = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, config, request.Transactions)
                .ConfigureAwait(false);

            var externalIds = await _services.GetValidGameActionExternalIdsAsync(gameProviderId, request.Transactions);

            var settleBatchResult = await ExecuteBatchAsync(request, gameProviderId, data, externalIds, true);
            var cancelBatchResult = await ExecuteBatchAsync(request, gameProviderId, data, externalIds, false);

            var results = new List<TransactionResponse>();
            results.AddRange(settleBatchResult);
            results.AddRange(cancelBatchResult);

            return new() {Results = results.ToArray()};
        }

        [HttpPost]
        [Route("freezewithdrawal")]
        public ValueTask<BaseResponse> FreezeAsync(FreezeRequest request) =>
            new(new BaseResponse());

        private async Task<IEnumerable<TransactionResponse>> ExecuteBatchAsync(BetRequest request, Guid gameProviderId,
            Dictionary<string, (Guid, string)> data,
            IDictionary<string, IEnumerable<GameActionExternalIds>> externalIds, bool isSettle)
        {
            var (batchBetData, notFoundTransactions) =
                _map.Request(gameProviderId, data, externalIds, request, isSettle);

            var transactionResponse = new List<TransactionResponse>();

            if (batchBetData.Transactions.Any())
            {
                var result = isSettle
                    ? await _actions.SettleBatchAsync(batchBetData).ConfigureAwait(false)
                    : await _actions.CancelBatchAsync(batchBetData).ConfigureAwait(false);

                transactionResponse.AddRange(_map.Response(result));
            }

            var notFounds = notFoundTransactions.ToArray();
            if (notFounds.Length > 0)
            {
                transactionResponse.AddRange(notFounds.Select(transactionId => new TransactionResponse
                {
                    ErrorCode = (int)GameApiError.TransactionIdNotFound,
                    ErrorMessage = "TransactionId is not found",
                    ExternalTransactionId = transactionId
                }));
            }

            return transactionResponse.ToArray();
        }

        private async Task<Dictionary<string, (Guid, string)>> GetPlayerGameProviderConfigurationIdsAsync(
            Guid gameProviderId,
            IMESportsConfiguration config, TransactionRequest[] transactions)
        {
            var map = new Dictionary<string, (Guid, string)>();

            foreach (var imPlayerCode in transactions.GroupBy(t => t.PlayerCode).Select(g => g.Key))
            {
                if (map.ContainsKey(imPlayerCode))
                {
                    continue;
                }

                var playerCode = await _services.GetPlayerCodeAsync(imPlayerCode, config);
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode)
                    .ConfigureAwait(false);

                var gameProviderConfigurationId =
                    await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId)
                        .ConfigureAwait(false);

                map.Add(imPlayerCode, (gameProviderConfigurationId, playerCode));
            }

            return map;
        }

        private async Task<IMSessionToken> ValidateSessionTokenAsync(string redisKey, string sessionToken)
        {
            var sessionTokenInRedis = await _cacheManager.SafeGetAsync(IMESportsConsts.RedisKey.SessionToken,
                redisKey, () => Task.FromResult(default(IMSessionToken)));

            if (sessionTokenInRedis?.SessionToken == null)
            {
                throw new PlayerLoginExpiredException("SessionToken in redis is empty");
            }

            if (sessionTokenInRedis.SessionToken.IgnoreCaseNotContains(sessionToken))
            {
                throw new PlayerLoginExpiredException(
                    $"SessionToken {sessionToken} in request different to SessionToken {sessionTokenInRedis.SessionToken} in redis");
            }

            return sessionTokenInRedis;
        }
    }
}
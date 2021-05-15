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
using TGS.Core.Models.Services.Data;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.IMESports.Common;
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
        private readonly IIMESportsActionModelMapper _map;
        private readonly IIMESportsServices _services;
        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IRedisCacheManager _cacheManager;

        public IMESportsGameController(
            IGamePlayActions actions,
            IIMESportsActionModelMapper map,
            IIMESportsServices services,
            IBrandOperations brandOperations,
            IRedisCacheManager cacheManager,
            IPlayerOperations playerOperations)
        {
            _actions = actions;
            _map = map;
            _services = services;
            _brandOperations = brandOperations;
            _cacheManager = cacheManager;
            _playerOperations = playerOperations;
        }

        [HttpPost]
        [Route("GetBalance")]
        public async ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var r = await _actions.GetBalanceAsync(GameProviderConst.IMESports, request.PlayerCode)
                .ConfigureAwait(false);

            return _map.Response(request.PlayerCode, r);
        }

        [HttpPost]
        [Route("PlaceBet")]
        public async ValueTask<BetResponse> PlaceBetAsync(BetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);

            var transaction = request.Transactions.First();
            var playerCode = transaction.PlayerCode;

            var sessionToken = await ValidateSessionTokenAsync(playerCode, request.SessionToken);
            
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            var gameProviderConfigurationId = bgpc.GameProviderConfigurationId;
            var platformType = transaction.Platform.IgnoreCaseEquals(PlatformType.Mobile.ToString()) ? PlatformType.Mobile : PlatformType.Desktop;

            var data = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = _map.Request(data, request, playerCode, allowPlaceBetWithDifferentPlatformToken: true);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
            
            await _cacheManager.SetKeyExpirationInAsync(IMESportsConsts.RedisKey.SessionToken, 
                playerCode, TimeSpan.FromSeconds(sessionToken.TimeToLeave));

            return _map.Response(r);
        }

        [HttpPost]
        [Route("SettleBet")]
        public async ValueTask<BetResponse> BatchAsync(BetRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            
            var data = await GetPlayerGameProviderConfigurationIdsAsync(gameProviderId, request.Transactions)
                .ConfigureAwait(false);

            var externalIds = await _services.GetValidGameActionExternalIdsAsync(gameProviderId, request.Transactions);
            
            var settleBatchResult = await ExecuteBatchAsync(request, gameProviderId, data, externalIds, isSettle: true);
            var cancelBatchResult = await ExecuteBatchAsync(request, gameProviderId, data, externalIds, isSettle: false);

            var results = new List<TransactionResponse>();
            results.AddRange(settleBatchResult);
            results.AddRange(cancelBatchResult);
            
            return new BetResponse
            {
                Results = results.ToArray()
            };
        }

        [HttpPost]
        [Route("FreezeWithdrawal")]
        public ValueTask<BaseResponse> FreezeAsync(FreezeRequest request)
        {
            return new (new BaseResponse());
        }

        private async Task<IEnumerable<TransactionResponse>> ExecuteBatchAsync(BetRequest request, Guid gameProviderId, Dictionary<string, Guid> data, 
            IDictionary<string, IEnumerable<GameActionExternalIds>> externalIds, bool isSettle)
        {
            var betData = _map.Request(gameProviderId, data, externalIds, request, isSettle);
            
            if (betData.Transactions.Any())
            {
                var result = isSettle ? 
                    await _actions.SettleBatchAsync(betData).ConfigureAwait(false) : 
                    await _actions.CancelBatchAsync(betData).ConfigureAwait(false);

                return _map.Response(result);
            }

            return Array.Empty<TransactionResponse>();
        }
        
        private async Task<Dictionary<string, Guid>> GetPlayerGameProviderConfigurationIdsAsync(Guid gameProviderId,
            TransactionRequest[] transactions)
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

        private async Task<IMSessionToken> ValidateSessionTokenAsync(string playerCode, string sessionToken)
        {
            var sessionTokenInRedis = await _cacheManager.SafeGetAsync(IMESportsConsts.RedisKey.SessionToken, 
                playerCode, () => Task.FromResult(default(IMSessionToken)));

            if (sessionTokenInRedis?.SessionToken == null)
            {
                throw new PlayerLoginExpiredException($"SessionToken in redis is empty");
            }
            
            if (sessionTokenInRedis.SessionToken.IgnoreCaseNotContains(sessionToken))
            {
                throw new PlayerLoginExpiredException($"SessionToken {sessionToken} in request different to SessionToken {sessionTokenInRedis.SessionToken} in redis");
            }

            return sessionTokenInRedis;
        }
    }
}
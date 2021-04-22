using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Saba.Attributes;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
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
        private readonly IRedisCacheManager _cacheManager;
        private readonly ISabaActionModelMapper _map;
        private readonly ISabaServices _services;

        public SabaGameController(
            IGamePlayActions actions,
            ISabaActionModelMapper map,
            ISabaServices services,
            IRedisCacheManager cacheManager)
        {
            _actions = actions;
            _map = map;
            _services = services;
            _cacheManager = cacheManager;
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
        [Route("confirmbet")]
        [ResolveRequest(Type = typeof(ConfirmBetRequest))]
        public async ValueTask<ConfirmBetResponse> ConfirmBetAsync(ConfirmBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            decimal unlockAmount;
            var confirmBet = request.Body.Transactions[0];

            if (confirmBet.IsOddsChanged)
            {
                var externalRoundId = confirmBet.ExternalRoundId.ToString();
                unlockAmount = await GetLockAmountAsync(playerCode, data.GameProviderId, externalRoundId)
                    .ConfigureAwait(false);
            }
            else
            {
                unlockAmount = confirmBet.ActualAmount;
            }

            var betData = _map.Request(playerCode, data, request, unlockAmount);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            await RemoveLockAmountCacheAsync(betData.Transactions).ConfigureAwait(false);

            return _map.Response(playerCode, request.Body, r);
        }

        [Route("confirmbet")]
        [ResolveRequest(Type = typeof(ConfirmBetRequest))]
        public async ValueTask<ConfirmBetResponse> ConfirmBetAsync(ConfirmBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            decimal unlockAmount;
            var confirmBet = request.Body.Transactions[0];

            if (confirmBet.IsOddsChanged)
            {
                var externalRoundId = confirmBet.ExternalRoundId.ToString();
                unlockAmount = await GetLockAmountAsync(playerCode, data.GameProviderId, externalRoundId)
                    .ConfigureAwait(false);
            }
            else
            {
                unlockAmount = confirmBet.ActualAmount;
            }

            var betData = _map.Request(playerCode, data, request, unlockAmount);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            await RemoveLockAmountCacheAsync(betData.Transactions).ConfigureAwait(false);

            return _map.Response(playerCode, request.Body, r);
        }

        [Route("cancelbet")]
        [ResolveRequest(Type = typeof(CancelBetRequest))]
        public async ValueTask<CancelBetResponse> ConfirmBetAsync(CancelBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            decimal unlockAmount;
            var confirmBet = request.Body.Transactions[0];

            if (confirmBet.IsOddsChanged)
            {
                var externalRoundId = confirmBet.ExternalRoundId.ToString();
                unlockAmount = await GetLockAmountAsync(playerCode, data.GameProviderId, externalRoundId)
                    .ConfigureAwait(false);
            }
            else
            {
                unlockAmount = confirmBet.ActualAmount;
            }

            var betData = _map.Request(playerCode, data, request, unlockAmount);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            await RemoveLockAmountCacheAsync(betData.Transactions).ConfigureAwait(false);

            return _map.Response(playerCode, request.Body, r);
        }


        private async ValueTask CacheLockAmountAsync(TransactionData[] transactions)
        {
            foreach (var t in transactions)
            {
                await _cacheManager.SafeCacheAsync(SabaConsts.RedisKey.LockAmount, t.ExternalRoundId,
                    TimeSpan.FromHours(12), t.Amount);
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
                Key = t.ExternalGameId
            }).ToArray();

            await _cacheManager.ClearCacheByBatchAsync(cacheKeys, true);
        }
    }
}
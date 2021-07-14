using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.AllBet.Data;
using TGS.Core.Infrastructures.Redis;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public interface IAllBetCacheServices
    {
        ValueTask CachePlayerCodeByExternalGameActionGroupIdAsync(string externalGameActionGroupId, string playerCode);

        ValueTask<string> GetPlayerCodeByExternalGameActionGroupIdAsync(string externalGameActionGroupId);

        ValueTask AddBetInfoToCacheAsync(string roundId, BetInfo info);

        ValueTask<BetInfo[]> GetBetInfosAsync(string roundId);
    }

    public class AllBetCacheServices : IAllBetCacheServices
    {
        private readonly IRedisCacheManager _cacheManager;

        public AllBetCacheServices(IRedisCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        async ValueTask IAllBetCacheServices.AddBetInfoToCacheAsync(string roundId, BetInfo info)
        {
            var betInfos = await _cacheManager.SafeGetAsync(
                AllBetConsts.RedisKey.BetInfos,
                roundId,
                () => Task.FromResult(new Dictionary<string, bool>())).ConfigureAwait(false);

            betInfos[info.BetNumber] = info.IsSettled;

            await _cacheManager.SafeCacheAsync(AllBetConsts.RedisKey.BetInfos,
                roundId, TimeSpan.FromDays(1), betInfos).ConfigureAwait(false);
        }

        async ValueTask IAllBetCacheServices.CachePlayerCodeByExternalGameActionGroupIdAsync(
            string externalGameActionGroupId, string playerCode)
            => await _cacheManager.SafeCacheAsync(
                AllBetConsts.RedisKey.PlayerCodeByTranId,
                externalGameActionGroupId,
                TimeSpan.FromDays(1),
                playerCode).ConfigureAwait(false);

        async ValueTask<BetInfo[]> IAllBetCacheServices.GetBetInfosAsync(string roundId)
        {
            var betInfos = await _cacheManager.SafeGetAsync(
                AllBetConsts.RedisKey.BetInfos,
                roundId,
                () => Task.FromResult(new Dictionary<string, bool>())).ConfigureAwait(false);

            return betInfos.Select(p => new BetInfo(p.Key, p.Value)).ToArray();
        }

        async ValueTask<string> IAllBetCacheServices.GetPlayerCodeByExternalGameActionGroupIdAsync(
            string externalGameActionGroupId)
            => await _cacheManager.SafeGetAsync(AllBetConsts.RedisKey.PlayerCodeByTranId,
                externalGameActionGroupId, () => Task.FromResult(string.Empty)).ConfigureAwait(false);
    }
}
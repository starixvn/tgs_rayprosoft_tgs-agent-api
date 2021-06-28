using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;

namespace TGS.Apis.TgsApi.GameProviders.AllBet
{
    public interface IAllBetCacheServices
    {
        ValueTask CachePlayerCodeByExternalGameActionGroupIdAsync(string externalGameActionGroupId, string playerCode);

        ValueTask<string> GetPlayerCodeByExternalGameActionGroupIdAsync(string externalGameActionGroupId);
    }

    public class AllBetCacheServices : IAllBetCacheServices
    {
        private readonly IRedisCacheManager _cacheManager;

        public AllBetCacheServices(IRedisCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        async ValueTask IAllBetCacheServices.CachePlayerCodeByExternalGameActionGroupIdAsync(
            string externalGameActionGroupId, string playerCode)
            => await _cacheManager.SafeCacheAsync(
                AllBetConsts.RedisKey.PlayerCodeByTranId,
                externalGameActionGroupId,
                TimeSpan.FromDays(1),
                playerCode).ConfigureAwait(false);

        async ValueTask<string> IAllBetCacheServices.GetPlayerCodeByExternalGameActionGroupIdAsync(
            string externalGameActionGroupId)
            => await _cacheManager.SafeGetAsync(AllBetConsts.RedisKey.PlayerCodeByTranId,
                externalGameActionGroupId, () => Task.FromResult(string.Empty)).ConfigureAwait(false);
    }
}
using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;

namespace TGS.Tests.Redis.IntegrationTests
{
    internal static class Extensions
    {
        internal static Task<T> GetAsync<T>(this IRedisCacheManager cacheManager, string cacheType, object cacheKey)
        {
            return ((RedisCacheManager) cacheManager).GetAsync<T>(cacheType, cacheKey);
        }

        internal static async Task CacheAsync<T>(this IRedisCacheManager cacheManager, string cacheType, object cacheKey,
            TimeSpan? expiresIn, T data)
        {
            await ((RedisCacheManager) cacheManager).CacheAsync(cacheType, cacheKey, expiresIn, data, false);
        }
    }
}

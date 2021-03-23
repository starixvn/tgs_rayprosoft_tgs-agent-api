using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public interface IRedisCacheManager
    {
        Task CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, List<T> data);

        Task CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data);

        Task<List<T>> GetListAsync<T>(object cacheType, object key, Func<Task<List<T>>> createFunc);

        Task ClearCacheAsync(object cacheType, object key, bool fireAndForget = false);

        Task<int> ClearCacheByBatchAsync(CacheKey[] keys, bool fireAndForget = false);

        /// <summary>
        /// Scans through all keys in redis database and deletes keys that matches some pattern.
        /// This pattern may contains special symbols like *?^ etc.
        /// More info about pattern building may be found here - https://redis.io/commands/keys.
        /// Please notice, this is heavy operation so should be used carefully.
        /// </summary>
        /// <param name="cacheType">Cache type object used as a key prefix and separates key spaces from one another</param>
        /// <param name="pattern">String that used as pattern to match keys against</param>
        /// <param name="fireAndForget">is fireforget or not </param>
        /// <returns>Amount of deleted keys</returns>

        Task<int> ClearCacheByPatternAsync(object cacheType, string pattern, bool fireAndForget = false);

        Task<T> SafeGetAndCacheAsync<T>(object cacheType, object key, TimeSpan? expiresIn, Func<Task<T>> getData);

        Task<T> SafeGetAsync<T>(object cacheType, object key, Func<Task<T>> getData);

        Task SafeCacheAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data);

        Task SetKeyExpirationInAsync(object cacheType, object key, TimeSpan expiresIn);

        Task<long> IncrementAsync(object cacheType, object key, long value = 1);

        Task<T> SafeGetAndCacheForeverAsync<T>(object cacheType, object key, Func<Task<T>> getData);

        Task SafeCacheForeverAsync<T>(object cacheType, object key, T data);

        Task SafeCacheHashSetAsync(object cacheType, object key, HashEntry[] hashFields, TimeSpan? expiresIn,
            bool fireAndForget = false);

        Task<RedisValue[]> SafeGetHashSetAsync(object cacheType, object key, RedisValue[] hashFields, bool fireAndForget = false);

        Task<RedisValue> SafeGetHashSetAsync(object cacheType, object key, RedisValue hashField, bool fireAndForget = false);

        Task<HashEntry[]> SafeGetAllHashSetAsync(object cacheType, object key, bool fireAndForget = false);
    }
}

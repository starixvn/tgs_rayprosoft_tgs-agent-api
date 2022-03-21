using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

[assembly: InternalsVisibleTo("TGS.Tests.Redis.IntegrationTests")]

namespace TGS.Core.Infrastructures.Redis
{
    internal sealed class RedisCacheManager : IRedisCacheManager
    {
        private static readonly TimeSpan _MaxTimeSpan = TimeSpan.FromDays(1);
        private readonly ILogger<RedisCacheManager> _logger;
        private readonly IRedisRepository _repository;

        public RedisCacheManager(
            IRedisRepository repository,
            ILogger<RedisCacheManager> logger)
        {
            _repository = repository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, List<T> data)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var existingKey = await db.KeyExistsAsync(cacheKey);
            var serializedValues = data.Select(d => (RedisValue)JsonSerializer.Serialize(d)).ToArray();
            await db.ListRightPushAsync(cacheKey, serializedValues);

            if (existingKey == false)
            {
                await db.KeyExpireAsync(cacheKey, CapExpiry(expiresIn));
            }
        }

        public Task CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data)
            => CacheListAsync(cacheType, key, expiresIn, new List<T> { data });

        public Task ClearCacheAsync(object cacheType, object key, bool fireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var flags = GetCommandFlags(fireAndForget);
            return db.KeyDeleteAsync(cacheKey, flags);
        }

        public async Task<int> ClearCacheByBatchAsync(CacheKey[] keys, bool fireAndForget)
        {
            if (keys == null || keys.Length == 0)
            {
                return 0;
            }

            var cacheKeys = keys.Select(k => (RedisKey)FormCacheKey(k)).ToArray();
            var db = _repository.GetDb();
            var flags = GetCommandFlags(fireAndForget);

            await db.KeyDeleteAsync(cacheKeys, flags);

            return keys.Length;
        }

        public async Task<int> ClearCacheByPatternAsync(object cacheType, string pattern,
            bool fireAndForget)
        {
            var keyPattern = FormCacheKey(cacheType, pattern);
            var db = _repository.GetDb();
            var multiplexer = db.Multiplexer;

            // because of replication all endpoints should contain same set of keys so taking any
            var endpoint = multiplexer.GetEndPoints().First();
            var server = multiplexer.GetServer(endpoint);

            var keys = server.Keys(pattern: keyPattern, database: db.Database).ToArray();
            var flags = GetCommandFlags(fireAndForget);

            var totalPage = Math.DivRem(keys.Length, 10000, out var rem);
            if (rem > 0)
                totalPage += 1;

            var index = 0;

            var deleteTasks = new List<Task>();
            while (index < totalPage)
            {
                var shard = keys.Skip(index * 10000).Take(10000).ToArray();
                deleteTasks.Add(db.KeyDeleteAsync(shard, flags));

                index++;
            }

            await Task.WhenAll(deleteTasks).ConfigureAwait(false);

            return keys.Length;
        }

        public async Task<List<T>> GetListAsync<T>(object cacheType, object key,
            Func<Task<List<T>>> createFunc)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var values = await db.ListRangeAsync(cacheKey);
            if (values.Length == 0)
                return await createFunc();

            return values.Select(v => JsonSerializer.Deserialize<T>(v)).ToList();
        }

        public async Task<RedisValue[]> GetSetAllMembersAsync(string cacheType, string key, bool fireAndForget = false)
        {
            try
            {
                var cacheKey = FormCacheKey(cacheType, key);
                var db = _repository.GetDb();

                var flags = GetCommandFlags(fireAndForget);
                return await db.SetMembersAsync(cacheKey, flags);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on key '{key}'",
                    key);
            }

            return Array.Empty<RedisValue>();
        }

        public Task<long> IncrementAsync(object cacheType, object key, long value)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            return db.StringIncrementAsync(cacheKey, value);
        }

        public async Task SafeCacheAsync<T>(
            object cacheType,
            object key,
            TimeSpan? expiresIn,
            T data)
        {
            try
            {
                await CacheAsync(cacheType, key, CapExpiry(expiresIn), data, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
            }
        }

        public async Task SafeCacheForeverAsync<T>(object cacheType, object key, T data)
        {
            try
            {
                await CacheAsync(cacheType, key, null, data, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
            }
        }

        public async Task SafeCacheHashSetAsync(object cacheType, object key, HashEntry[] hashFields,
            TimeSpan? expiresIn, bool fireAndForget)
        {
            try
            {
                var cacheKey = FormCacheKey(cacheType, key);
                var db = _repository.GetDb();

                await db.HashSetAsync(cacheKey, hashFields);

                var flags = GetCommandFlags(fireAndForget);
                await db.KeyExpireAsync(cacheKey, expiresIn, flags);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
            }
        }

        public async Task<HashEntry[]> SafeGetAllHashSetAsync(object cacheType, object key,
            bool fireAndForget)
        {
            try
            {
                var cacheKey = FormCacheKey(cacheType, key);
                var db = _repository.GetDb();

                var flags = GetCommandFlags(fireAndForget);
                return await db.HashGetAllAsync(cacheKey, flags);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
            }

            return Array.Empty<HashEntry>();
        }

        public async Task<T> SafeGetAndCacheAsync<T>(
            object cacheType,
            object key,
            TimeSpan? expiresIn,
            Func<Task<T>> getData)
        {
            try
            {
                return await GetAndCacheAsync(cacheType, key, CapExpiry(expiresIn), getData, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
                return await getData();
            }
        }

        public async Task<T> SafeGetAndCacheForeverAsync<T>(object cacheType, object key,
            Func<Task<T>> getData)
        {
            try
            {
                return await GetAndCacheAsync(cacheType, key, null, getData, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
                return await getData();
            }
        }

        public async Task<T> SafeGetAsync<T>(
            object cacheType,
            object key,
            Func<Task<T>> getData)
        {
            try
            {
                return await GetAsync(cacheType, key, getData);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
                return await getData();
            }
        }

        public async Task<RedisValue[]> SafeGetHashSetAsync(object cacheType, object key,
            RedisValue[] hashFields,
            bool fireAndForget)
        {
            try
            {
                var cacheKey = FormCacheKey(cacheType, key);
                var db = _repository.GetDb();

                var flags = GetCommandFlags(fireAndForget);
                return await db.HashGetAsync(cacheKey, hashFields, flags);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
            }

            return Array.Empty<RedisValue>();
        }

        public async Task<RedisValue> SafeGetHashSetAsync(object cacheType, object key,
            RedisValue hashField, bool fireAndForget)
        {
            try
            {
                var cacheKey = FormCacheKey(cacheType, key);
                var db = _repository.GetDb();

                var flags = GetCommandFlags(fireAndForget);
                return await db.HashGetAsync(cacheKey, hashField, flags);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on type '{cacheType}' with key '{key}'",
                    cacheType,
                    key);
            }

            return RedisValue.Null;
        }

        public async Task<long> SetAddAsync(string cacheType, string key, RedisValue[] values,
            TimeSpan? expiresIn,
            bool fireAndForget)
        {
            try
            {
                var cacheKey = FormCacheKey(cacheType, key);
                var db = _repository.GetDb();

                var flags = GetCommandFlags(fireAndForget);
                var count = await db.SetAddAsync(cacheKey, values, flags);
                if (expiresIn != null)
                {
                    await db.KeyExpireAsync(cacheKey, CapExpiry(expiresIn));
                }

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "redis error on key '{key}'",
                    key);
            }

            return 0;
        }

        public Task SetKeyExpirationInAsync(object cacheType, object key, TimeSpan expiresIn)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            return db.KeyExpireAsync(cacheKey, expiresIn);
        }

        private static CommandFlags GetCommandFlags(bool fireAndForget)
            => fireAndForget ? CommandFlags.FireAndForget : CommandFlags.None;

        #region Internal Methods

        internal static TimeSpan CapExpiry(TimeSpan? expiresIn) =>
            expiresIn == null || expiresIn.Value > _MaxTimeSpan ? _MaxTimeSpan : expiresIn.Value;

        internal static string FormCacheKey(CacheKey cacheKey) =>
            FormCacheKey(cacheKey.CacheType, cacheKey.Key);

        internal static string FormCacheKey(object cacheType, object key) =>
            $"{cacheType}:{key}";

        internal Task CacheAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data,
            bool cacheWithFireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var serializedValue = JsonSerializer.Serialize(data);
            var flags = GetCommandFlags(cacheWithFireAndForget);
            return db.StringSetAsync(cacheKey, serializedValue, expiresIn, When.Always, flags);
        }

        internal async Task<T> GetAndCacheAsync<T>(
            object cacheType, object key, TimeSpan? expiresIn, Func<Task<T>> createFunc, bool cacheWithFireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var value = await db.StringGetAsync(cacheKey);

            if (!value.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            var createdValue = await createFunc();
            var serializedValue = JsonSerializer.Serialize(createdValue);
            var flags = GetCommandFlags(cacheWithFireAndForget);
            await db.StringSetAsync(cacheKey, serializedValue, expiresIn, When.Always, flags);
            return createdValue;
        }

        internal async Task<T> GetAsync<T>(object cacheType, object key, Func<Task<T>> createFunc = null)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var value = await db.StringGetAsync(cacheKey);

            if (value.IsNullOrEmpty)
            {
                return createFunc != null ? await createFunc() : default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }

        #endregion Internal Methods
    }
}

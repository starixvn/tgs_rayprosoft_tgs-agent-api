using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Json;

[assembly: InternalsVisibleTo("TGS.Tests.Redis.IntegrationTests")]

namespace TGS.Core.Infrastructures.Redis
{
    public sealed class RedisCacheManager : IRedisCacheManager
    {
        internal static readonly TimeSpan MaxTimeSpan = TimeSpan.FromDays(1);
        private readonly IRedisCacheManager _interface;
        private readonly IRedisRepository _repository;
        private readonly IJsonSerializationProvider _serializationProvider;

        public RedisCacheManager(
            IRedisRepository repository,
            IJsonSerializationProvider serializationProvider)
        {
            _repository = repository;
            _serializationProvider = serializationProvider;
            _interface = this;
        }

        async Task IRedisCacheManager.CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, List<T> data)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var existingKey = await db.KeyExistsAsync(cacheKey);
            var serializedValues = data.Select(d => (RedisValue) _serializationProvider.SerializeToString(d)).ToArray();
            await db.ListRightPushAsync(cacheKey, serializedValues);

            if (existingKey == false)
            {
                await db.KeyExpireAsync(cacheKey, CapExpiry(expiresIn));
            }
        }

        Task IRedisCacheManager.CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data)
        {
            return _interface.CacheListAsync(cacheType, key, expiresIn, new List<T> {data});
        }

        async Task<List<T>> IRedisCacheManager.GetListAsync<T>(object cacheType, object key,
            Func<Task<List<T>>> createFunc)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var values = await db.ListRangeAsync(cacheKey);
            if (values.Length == 0)
                return await createFunc();

            return values.Select(v => _serializationProvider.DeserializeFromString<T>(v)).ToList();
        }

        Task IRedisCacheManager.ClearCacheAsync(object cacheType, object key, bool fireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var flags = GetCommandFlags(fireAndForget);
            return db.KeyDeleteAsync(cacheKey, flags);
        }

        async Task<int> IRedisCacheManager.ClearCacheByBatchAsync(CacheKey[] keys, bool fireAndForget)
        {
            if (keys == null || keys.Length == 0)
            {
                return 0;
            }

            var cacheKeys = keys.Select(k => (RedisKey) FormCacheKey(k)).ToArray();
            var db = _repository.GetDb();
            var flags = GetCommandFlags(fireAndForget);

            await db.KeyDeleteAsync(cacheKeys, flags);

            return keys.Length;
        }

        async Task<int> IRedisCacheManager.ClearCacheByPatternAsync(object cacheType, string pattern,
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

            await db.KeyDeleteAsync(keys.ToArray(), flags);

            return keys.Length;
        }

        async Task<T> IRedisCacheManager.SafeGetAndCacheAsync<T>(
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
                Console.WriteLine($"redis error :{ex}");
                return await getData();
            }
        }

        async Task<T> IRedisCacheManager.SafeGetAsync<T>(
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
                Console.WriteLine($"redis error :{ex}");
                return await getData();
            }
        }

        async Task IRedisCacheManager.SafeCacheAsync<T>(
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
                Console.WriteLine($"redis error :{ex}");
            }
        }

        Task IRedisCacheManager.SetKeyExpirationInAsync(object cacheType, object key, TimeSpan expiresIn)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            return db.KeyExpireAsync(cacheKey, expiresIn);
        }

        Task<long> IRedisCacheManager.IncrementAsync(object cacheType, object key, long value)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            return db.StringIncrementAsync(cacheKey, value);
        }

        async Task<T> IRedisCacheManager.SafeGetAndCacheForeverAsync<T>(object cacheType, object key,
            Func<Task<T>> getData)
        {
            try
            {
                return await GetAndCacheAsync(cacheType, key, null, getData, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"redis error :{ex}");
                return await getData();
            }
        }

        async Task IRedisCacheManager.SafeCacheForeverAsync<T>(object cacheType, object key, T data)
        {
            try
            {
                await CacheAsync(cacheType, key, null, data, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"redis error :{ex}");
            }
        }

        async Task IRedisCacheManager.SafeCacheHashSetAsync(object cacheType, object key, HashEntry[] hashFields,
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
                Console.WriteLine($"redis error :{ex}");
            }
        }

        async Task<RedisValue[]> IRedisCacheManager.SafeGetHashSetAsync(object cacheType, object key,
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
                Console.WriteLine($"redis error :{ex}");
            }

            return Array.Empty<RedisValue>();
        }

        async Task<HashEntry[]> IRedisCacheManager.SafeGetAllHashSetAsync(object cacheType, object key,
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
                Console.WriteLine($"redis error :{ex}");
            }

            return Array.Empty<HashEntry>();
        }

        async Task<RedisValue> IRedisCacheManager.SafeGetHashSetAsync(object cacheType, object key,
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
                Console.WriteLine($"redis error :{ex}");
            }

            return RedisValue.Null;
        }

        private static CommandFlags GetCommandFlags(bool fireAndForget)
        {
            return fireAndForget ? CommandFlags.FireAndForget : CommandFlags.None;
        }

        #region Internal Methods

        internal static string FormCacheKey(CacheKey cacheKey) =>
            FormCacheKey(cacheKey.CacheType, cacheKey.Key);

        internal static string FormCacheKey(object cacheType, object key) =>
            $"{cacheType}:{key}";

        internal static TimeSpan CapExpiry(TimeSpan? expiresIn) =>
            expiresIn == null || expiresIn.Value > MaxTimeSpan ? MaxTimeSpan : expiresIn.Value;

        internal async Task<T> GetAsync<T>(object cacheType, object key, Func<Task<T>> createFunc = null)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var value = await db.StringGetAsync(cacheKey);

            if (value.IsNullOrEmpty)
            {
                return createFunc != null ? await createFunc() : default;
            }

            return _serializationProvider.DeserializeFromString<T>(value);
        }

        internal async Task<T> GetAndCacheAsync<T>(
            object cacheType, object key, TimeSpan? expiresIn, Func<Task<T>> createFunc, bool cacheWithFireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var value = await db.StringGetAsync(cacheKey);

            if (!value.IsNullOrEmpty)
            {
                return _serializationProvider.DeserializeFromString<T>(value);
            }

            var createdValue = await createFunc();
            var serializedValue = _serializationProvider.SerializeToString(createdValue);
            var flags = GetCommandFlags(cacheWithFireAndForget);
            await db.StringSetAsync(cacheKey, serializedValue, expiresIn, When.Always, flags);
            return createdValue;
        }

        internal Task CacheAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data,
            bool cacheWithFireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var serializedValue = _serializationProvider.SerializeToString(data);
            var flags = GetCommandFlags(cacheWithFireAndForget);
            return db.StringSetAsync(cacheKey, serializedValue, expiresIn, When.Always, flags);
        }

        #endregion Internal Methods
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Log;

[assembly:InternalsVisibleTo("TGS.Tests.Redis.IntegrationTests")]
namespace TGS.Infrastructures.Redis
{
    public sealed class RedisCacheManager : IRedisCacheManager
    {
        private readonly IRedisCacheManager _interface;
        private readonly IRedisRepository _repository;
        private readonly IJsonSerializationProvider _serializationProvider;
        internal static readonly TimeSpan MaxTimeSpan = TimeSpan.FromDays(1);
        private readonly IOsLog _osLog;

        public RedisCacheManager(IRedisRepository repository, IJsonSerializationProvider serializationProvider, IOsLog osLog)
        {
            _repository = repository;
            _serializationProvider = serializationProvider;
            _osLog = osLog;
            _interface = this;
        }

        async Task IRedisCacheManager.CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, List<T> data)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var existingKey = await db.KeyExistsAsync(cacheKey);
            var serializedValues = data.Select(d => (RedisValue)_serializationProvider.SerializeToString(d)).ToArray();
            await db.ListRightPushAsync(cacheKey, serializedValues);
            if (existingKey == false)
            {
                await db.KeyExpireAsync(cacheKey, CapExpiry(expiresIn));
            }
        }

        Task IRedisCacheManager.CacheListAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data)
        {
            return _interface.CacheListAsync(cacheType, key, expiresIn, new List<T> { data });
        }

        async Task<List<T>> IRedisCacheManager.GetListAsync<T>(object cacheType, object key, Func<Task<List<T>>> createFunc)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var values = await db.ListRangeAsync(cacheKey);
            if (values.Length == 0)
            {
                return await createFunc();
            }

            return values.Select(v => _serializationProvider.DeserializeFromString<T>(v)).ToList();
        }

        Task IRedisCacheManager.ClearCacheAsync(object cacheType, object key, bool fireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            return db.KeyDeleteAsync(cacheKey, fireAndForget ? CommandFlags.FireAndForget : CommandFlags.None);
        }

        async Task<int> IRedisCacheManager.ClearCacheByPatternAsync(object cacheType, string pattern, bool fireAndForget)
        {
            var keyPattern = FormCacheKey(cacheType, pattern);
            var db = _repository.GetDb();
            var multiplexer = db.Multiplexer;

            // because of replication all endpoints should contain same set of keys so taking any
            var endpoint = multiplexer.GetEndPoints().First();
            var server = multiplexer.GetServer(endpoint);

            var count = 0;
            var keys = server.Keys(pattern: keyPattern, database: db.Database);
            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key, fireAndForget ? CommandFlags.FireAndForget : CommandFlags.None);
                count++;
            }
            return count;
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
                _osLog.WriteEventLogError($"redis error :{ex}");
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
                _osLog.WriteEventLogError($"redis error :{ex}");
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
                _osLog.WriteEventLogError($"redis error :{ex}");
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

        async Task<T> IRedisCacheManager.SafeGetAndCacheForeverAsync<T>(object cacheType, object key, Func<Task<T>> getData)
        {
            try
            {
                return await GetAndCacheAsync(cacheType, key, null, getData, true);
            }
            catch (Exception ex)
            {
                _osLog.WriteEventLogError($"redis error :{ex}");
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
                _osLog.WriteEventLogError($"redis error :{ex}");
            }
        }

        #region Internal Methods

        internal static string FormCacheKey(object cacheType, object key)
        {
            return $"{cacheType}:{key}";
        }

        internal static TimeSpan CapExpiry(TimeSpan? expiresIn)
        {
            return expiresIn == null || expiresIn.Value > MaxTimeSpan ? MaxTimeSpan : expiresIn.Value;
        }

        internal async Task<T> GetAsync<T>(object cacheType, object key, Func<Task<T>> createFunc = null)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var value = await db.StringGetAsync(cacheKey);

            if (value.IsNullOrEmpty) 
                return createFunc != null ? await createFunc() : default;

            return _serializationProvider.DeserializeFromString<T>(value);
        }

        // This method presents as a temporary measure to check FireAndForget as Jeff suggested
        // If this proves its good then this method should be merged back to IRedisCacheManager.CacheAsync
        // and cacheWithFireAndForget replaced with just passing CommandFlags.FireAndForget
        internal async Task<T> GetAndCacheAsync<T>(
            object cacheType, object key, TimeSpan? expiresIn, Func<Task<T>> createFunc, bool cacheWithFireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var value = await db.StringGetAsync(cacheKey);
            
            if (!value.IsNullOrEmpty) 
                return _serializationProvider.DeserializeFromString<T>(value);

            var createdValue = await createFunc();
            var serializedValue = _serializationProvider.SerializeToString(createdValue);
            var flags = cacheWithFireAndForget ? CommandFlags.None : CommandFlags.FireAndForget;
            await db.StringSetAsync(cacheKey, serializedValue, expiresIn, When.Always, flags);
            return createdValue;
        }

        // This method presents as a temporary measure to check FireAndForget as Jeff suggested
        // If this proves its good then this method should be merged back to IRedisCacheManager.CacheAsync
        // and cacheWithFireAndForget replaced with just passing CommandFlags.FireAndForget
        internal Task CacheAsync<T>(object cacheType, object key, TimeSpan? expiresIn, T data, bool cacheWithFireAndForget)
        {
            var cacheKey = FormCacheKey(cacheType, key);
            var db = _repository.GetDb();
            var serializedValue = _serializationProvider.SerializeToString(data);
            var flags = cacheWithFireAndForget ? CommandFlags.None : CommandFlags.FireAndForget;
            return db.StringSetAsync(cacheKey, serializedValue, expiresIn, When.Always, flags);
        }

        #endregion
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Json;

namespace TGS.Core.Infrastructures.Redis
{
    public sealed class RedisList<T> : IRedisList<T>
    {
        private readonly IRedisList<T> _interface;
        private readonly IRedisRepository _repository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private string _keyPrefix;

        public RedisList(
            IRedisRepository repository,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _interface = this;
            _repository = repository;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        void IRedisList<T>.SetKeyPrefix(string prefix)
        {
            _keyPrefix = prefix;
        }

        Task IRedisList<T>.AddToStartAsync(T item)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var value = _jsonSerializationProvider.SerializeToString(item);
            return db.ListLeftPushAsync(key, value);
        }

        Task IRedisList<T>.AddToStartAsync(T[] items)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var values = items.Select(x => (RedisValue)_jsonSerializationProvider.SerializeToString(x)).ToArray();
            return db.ListLeftPushAsync(key, values);
        }

        Task IRedisList<T>.AddToEndAsync(T item)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var value = _jsonSerializationProvider.SerializeToString(item);
            return db.ListRightPushAsync(key, value);
        }

        Task IRedisList<T>.AddToEndAsync(T[] items)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var values = items.Select(x => (RedisValue)_jsonSerializationProvider.SerializeToString(x)).ToArray();
            return db.ListRightPushAsync(key, values);
        }

        async Task<T> IRedisList<T>.PopFromStartAsync()
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            string value = await db.ListLeftPopAsync(key);
            return string.IsNullOrEmpty(value)
                ? default(T)
                : _jsonSerializationProvider.DeserializeFromString<T>(value);
        }

        async Task<T> IRedisList<T>.PopFromEndAsync()
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            string value = await db.ListRightPopAsync(key);
            return string.IsNullOrEmpty(value)
                 ? default(T)
                 : _jsonSerializationProvider.DeserializeFromString<T>(value);
        }

        Task IRedisList<T>.ShrinkAsync(long maxSize)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            return db.ListTrimAsync(key, 0, maxSize - 1);
        }

        Task<long> IRedisList<T>.CountAsync()
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            return db.ListLengthAsync(key);
        }

        Task<T[]> IRedisList<T>.TakeAsync(long count, long skip)
        {
            return _interface.GetRangeAsync(skip, count);
        }

        async Task<T[]> IRedisList<T>.GetRangeAsync(long start, long count)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var range = await db.ListRangeAsync(key, start, start + count - 1);
            if (range == null)
            {
                return new T[0];
            }

            return range
                .Select(x => _jsonSerializationProvider.DeserializeFromString<T>(x))
                .ToArray();
        }

        Task IRedisList<T>.ClearAsync()
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            return db.KeyDeleteAsync(key);
        }

        string IRedisList<T>.GetKey()
        {
            if (string.IsNullOrEmpty(_keyPrefix))
            {
                throw new ArgumentNullException(nameof(_keyPrefix), "Key prefix should be set before using actual methods");
            }

            return $"{_keyPrefix}_{GetType().Name}_{typeof(T).Name}";
        }
    }
}
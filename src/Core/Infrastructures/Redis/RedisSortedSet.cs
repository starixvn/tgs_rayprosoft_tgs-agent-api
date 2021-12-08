using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    internal sealed class RedisSortedSet<T> : IRedisSortedSet<T>
    {
        private readonly IRedisSortedSet<T> _interface;
        private readonly IRedisRepository _repository;
        private string _keyPrefix;

        public RedisSortedSet(
            IRedisRepository repository)
        {
            _interface = this;
            _repository = repository;
        }

        Task IRedisSortedSet<T>.AddAsync(double score, T item)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var value = JsonSerializer.Serialize(item);
            return db.SortedSetAddAsync(key, value, score);
        }

        Task IRedisSortedSet<T>.AddRangeAsync(IEnumerable<ScoredItem<T>> items)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var values = items.Select(x =>
                {
                    var value = JsonSerializer.Serialize(x.Item);
                    return new SortedSetEntry(value, x.Score);
                })
                .ToArray();

            return db.SortedSetAddAsync(key, values);
        }

        async Task<int> IRedisSortedSet<T>.CountAsync()
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var length = await db.SortedSetLengthAsync(key);
            return ToInt(length);
        }

        string IRedisSortedSet<T>.GetKey()
        {
            if (string.IsNullOrEmpty(_keyPrefix))
            {
                throw new ArgumentNullException(nameof(_keyPrefix), "Key prefix should be set before using actual methods");
            }

            return $"{_keyPrefix}_{GetType().Name}_{typeof(T).Name}";
        }

        async Task<T> IRedisSortedSet<T>.GetWithMaxScoreAsync()
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var items = await db.SortedSetRangeByRankAsync(key, 0, 0, Order.Descending);

            if (items == null || items.Length != 1)
            {
                return default(T);
            }
            return DeserializeItem(items[0]);
        }

        async Task<T> IRedisSortedSet<T>.GetWithMinScoreAsync()
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var items = await db.SortedSetRangeByRankAsync(key, 0, 0, Order.Ascending);

            if (items == null || items.Length != 1)
            {
                return default(T);
            }
            return DeserializeItem(items[0]);
        }

        void IRedisSortedSet<T>.SetKeyPrefix(string prefix)
        {
            _keyPrefix = prefix;
        }

        async Task<T[]> IRedisSortedSet<T>.TakeAsync(int count)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var items = await db.SortedSetRangeByRankAsync(key, 0, count - 1, Order.Ascending);

            return items
                .Select(DeserializeItem)
                .ToArray();
        }

        async Task<T[]> IRedisSortedSet<T>.TakeDescendingAsync(int count)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            var items = await db.SortedSetRangeByRankAsync(key, 0, count - 1, Order.Descending);

            return items
                .Select(DeserializeItem)
                .ToArray();
        }

        Task IRedisSortedSet<T>.TrimLowerAsync(int targetLength)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            return db.SortedSetRemoveRangeByRankAsync(key, 0, -targetLength - 1);
        }

        Task IRedisSortedSet<T>.TrimUpperAsync(int targetLength)
        {
            var key = _interface.GetKey();
            var db = _repository.GetDb();

            return db.SortedSetRemoveRangeByRankAsync(key, targetLength, -1);
        }

        private T DeserializeItem(RedisValue item)
        {
            return JsonSerializer.Deserialize<T>(item);
        }

        private int ToInt(long value)
        {
            return (int)value;
        }
    }
}

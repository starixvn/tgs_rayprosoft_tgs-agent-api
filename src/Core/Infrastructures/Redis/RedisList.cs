using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis;

internal class RedisList<T> : IRedisList<T>
{
    private readonly IRedisList<T> _interface;
    private readonly IRedisRepository _repository;
    private string _keyPrefix;

    public RedisList(
        IRedisRepository repository)
    {
        _interface = this;
        _repository = repository;
    }

    Task IRedisList<T>.AddToEndAsync(T item)
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        var value = JsonSerializer.Serialize(item);
        return db.ListRightPushAsync(key, value);
    }

    Task IRedisList<T>.AddToEndAsync(T[] items)
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        var values = items.Select(x => (RedisValue)JsonSerializer.Serialize(x)).ToArray();
        return db.ListRightPushAsync(key, values);
    }

    Task IRedisList<T>.AddToStartAsync(T item)
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        var value = JsonSerializer.Serialize(item);
        return db.ListLeftPushAsync(key, value);
    }

    Task IRedisList<T>.AddToStartAsync(T[] items)
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        var values = items.Select(x => (RedisValue)JsonSerializer.Serialize(x)).ToArray();
        return db.ListLeftPushAsync(key, values);
    }

    Task IRedisList<T>.ClearAsync()
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        return db.KeyDeleteAsync(key);
    }

    Task<long> IRedisList<T>.CountAsync()
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        return db.ListLengthAsync(key);
    }

    string IRedisList<T>.GetKey()
    {
        if (string.IsNullOrEmpty(_keyPrefix))
        {
            throw new ArgumentNullException(nameof(_keyPrefix), "Key prefix should be set before using actual methods");
        }

        return $"{_keyPrefix}_{GetType().Name}_{typeof(T).Name}";
    }

    async Task<T[]> IRedisList<T>.GetRangeAsync(long start, long count)
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        var range = await db.ListRangeAsync(key, start, start + count - 1);
        if (range == null)
        {
            return Array.Empty<T>();
        }

        return range
            .Select(x => JsonSerializer.Deserialize<T>(x))
            .ToArray();
    }

    async Task<T> IRedisList<T>.PopFromEndAsync()
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        string value = await db.ListRightPopAsync(key);
        return string.IsNullOrEmpty(value)
             ? default
             : JsonSerializer.Deserialize<T>(value);
    }

    async Task<T> IRedisList<T>.PopFromStartAsync()
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        string value = await db.ListLeftPopAsync(key);
        return string.IsNullOrEmpty(value)
            ? default
            : JsonSerializer.Deserialize<T>(value);
    }

    void IRedisList<T>.SetKeyPrefix(string prefix)
    {
        _keyPrefix = prefix;
    }

    Task IRedisList<T>.ShrinkAsync(long maxSize)
    {
        var key = _interface.GetKey();
        var db = _repository.GetDb();

        return db.ListTrimAsync(key, 0, maxSize - 1);
    }

    Task<T[]> IRedisList<T>.TakeAsync(long count, long skip)
    {
        return _interface.GetRangeAsync(skip, count);
    }
}

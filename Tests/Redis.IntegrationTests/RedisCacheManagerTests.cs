using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace TGS.Infrastructures.Redis.IntegrationTests
{
    [TestFixture, Parallelizable]
    [Category(Named.Integration)]
    public partial class RedisCacheManagerTests
    {
        private const string CacheType = "DataByGuid";
        private Guid _cacheKey;
        private const string NewData = "New data";

        private Func<string, IRedisRepository> _redisRepositoryFactory;
        private Func<string, IRedisCacheManager> _cacheManagerFactory;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var provider = new ServiceCollection()
                .AddRedisServices()
                .BuildServiceProvider();

            _redisRepositoryFactory = provider.GetService<Func<string, IRedisRepository>>();
            _cacheManagerFactory = provider.GetService<Func<string, IRedisCacheManager>>();
        }

        [SetUp]
        public void SetUp()
        {
            _cacheKey = Guid.NewGuid();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var type in Enum.GetNames(typeof(RedisConnectionType)))
            {
                var amount = AsyncHelper.RunSync(() =>
                    _cacheManagerFactory(type).ClearCacheByPatternAsync(CacheType, "*"));
                Console.WriteLine($"TearDown removed {amount} keys");
            }
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Get_returns_value_if_it_is_in_cache(RedisConnectionType type)
        {
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, _cacheKey, null, NewData);

            var dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(NewData);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Get_returns_default_if_it_is_not_in_cache(RedisConnectionType type)
        {
            var dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(default(string));
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Get_creates_data_if_it_is_not_in_cache(RedisConnectionType type)
        {
            var dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync(CacheType, _cacheKey, () => Task.FromResult(NewData));
            dataFromCache.Should().Be(NewData);

            dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(default(string));
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task GetAndCache_creates_and_caches_data_if_it_is_not_in_cache(RedisConnectionType type)
        {
            var dataFromCache = await _cacheManagerFactory(type.ToString()).GetAndCacheAsync(CacheType, _cacheKey, null, () => Task.FromResult(NewData));
            dataFromCache.Should().Be(NewData);

            dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(NewData);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Cache_caches_data_if_it_is_not_in_cache(RedisConnectionType type)
        {
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, _cacheKey, null, NewData);

            var dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(NewData);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Cache_replaces_data_if_it_is_in_cache(RedisConnectionType type)
        {
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, _cacheKey, null, NewData);

            var rewriteData = "Rewrite data";
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, _cacheKey, null, rewriteData);

            var dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(rewriteData);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task CacheList_caches_data_if_it_is_not_in_cache(RedisConnectionType type)
        {
            await _cacheManagerFactory(type.ToString()).CacheListAsync(CacheType, _cacheKey, null, NewData);

            var listFromCache = await _cacheManagerFactory(type.ToString()).GetListAsync(CacheType, _cacheKey, () => Task.FromResult(new List<string>()));
            listFromCache.Count.Should().Be(1);
            listFromCache[0].Should().Be(NewData);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task CacheList_pushes_data_if_it_is_in_cache(RedisConnectionType type)
        {
            await _cacheManagerFactory(type.ToString()).CacheListAsync(CacheType, _cacheKey, null, NewData);
            await _cacheManagerFactory(type.ToString()).CacheListAsync(CacheType, _cacheKey, null, NewData);

            var listFromCache = await _cacheManagerFactory(type.ToString()).GetListAsync(CacheType, _cacheKey, () => Task.FromResult(new List<string>()));
            listFromCache.Count.Should().Be(2);
            listFromCache[0].Should().Be(NewData);
            listFromCache[1].Should().Be(NewData);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task GetList_creates_data_if_it_is_not_in_cache(RedisConnectionType type)
        {
            var createdList = await _cacheManagerFactory(type.ToString()).GetListAsync(CacheType, _cacheKey, () => Task.FromResult(new List<string> { NewData, NewData }));
            createdList.Count.Should().Be(2);
            createdList[0].Should().Be(NewData);
            createdList[1].Should().Be(NewData);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task ClearCache_clears_cache(RedisConnectionType type)
        {
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, _cacheKey, null, NewData);
            await _cacheManagerFactory(type.ToString()).ClearCacheAsync(CacheType, _cacheKey);

            var dataFromCache = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(default(string));
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Set_expireIn_sets_expire(RedisConnectionType type)
        {
            var key = Guid.NewGuid();
            var cacheKey = RedisCacheManager.FormCacheKey(CacheType, key);

            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, key, null, NewData);
            var database = _redisRepositoryFactory(type.ToString()).GetDb();
            var value = await database.StringGetWithExpiryAsync(cacheKey);
            var expiresIn = RedisCacheManager.CapExpiry(null);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);

            expiresIn = TimeSpan.FromSeconds(20);
            await _cacheManagerFactory(type.ToString()).SetKeyExpirationInAsync(CacheType, key, expiresIn);
            value = await database.StringGetWithExpiryAsync(cacheKey);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Set_expireIn_changes_expire(RedisConnectionType type)
        {
            var key = Guid.NewGuid();
            var cacheKey = $"{CacheType}:{key}";

            var expiresIn = TimeSpan.FromSeconds(20);
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, key, expiresIn, NewData);
            var database = _redisRepositoryFactory(type.ToString()).GetDb();
            var value = await database.StringGetWithExpiryAsync(cacheKey);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);

            expiresIn = TimeSpan.FromSeconds(30);
            await _cacheManagerFactory(type.ToString()).SetKeyExpirationInAsync(CacheType, key, expiresIn);
            value = await database.StringGetWithExpiryAsync(cacheKey);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task ClearCacheByPattern_clear_right_keys(RedisConnectionType type)
        {
            var key11 = "1:11";
            var key12 = "1:12";
            var key21 = "1:21";
            var key22 = "1:22";

            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, key11, null, NewData);
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, key12, null, NewData);
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, key21, null, NewData);
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, key22, null, NewData);

            var amount = await _cacheManagerFactory(type.ToString()).ClearCacheByPatternAsync(CacheType, "1:2*");

            var key11Value = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, key11);
            var key12Value = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, key12);
            var key21Value = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, key21);
            var key22Value = await _cacheManagerFactory(type.ToString()).GetAsync<string>(CacheType, key22);

            amount.Should().Be(2);
            key11Value.Should().Be(NewData);
            key12Value.Should().Be(NewData);
            key21Value.Should().Be(default(string));
            key22Value.Should().Be(default(string));
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_ClearCacheByPattern_when_no_matches(RedisConnectionType type)
        {
            var amount = await _cacheManagerFactory(type.ToString()).ClearCacheByPatternAsync(CacheType, $"{Guid.NewGuid()}*");
            amount.Should().Be(0);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_ClearCacheByPattern_when_lots_of_matches(RedisConnectionType type)
        {
            for (var i = 0; i < 2000; i++)
            {
                await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, $"pattern:{i % 2}:{i:D4}", null, NewData);
            }
            var amount = await _cacheManagerFactory(type.ToString()).ClearCacheByPatternAsync(CacheType, "pattern:0:*");
            amount.Should().Be(1000);
        }
    }
}
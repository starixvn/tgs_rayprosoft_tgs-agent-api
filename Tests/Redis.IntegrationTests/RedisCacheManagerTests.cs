using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TGS.Core.Infrastructures.Redis;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Redis.IntegrationTests
{
    [TestFixture]
    [Parallelizable]
    [Category(Named.Integration)]
    public partial class RedisCacheManagerTests
    {
        private const string CacheType = "DataByGuid";
        private Guid _cacheKey;
        private const string NewData = "New data";

        private IRedisRepository _redisRepository;
        private IRedisCacheManager _cacheManager;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var provider = new ServiceCollection()
                .AddConfiguration()
                .AddRedis()
                .BuildServiceProvider();

            _redisRepository = provider.GetService<IRedisRepository>();
            _cacheManager = provider.GetService<IRedisCacheManager>();
        }

        [SetUp]
        public void SetUp()
        {
            _cacheKey = Guid.NewGuid();
        }

        [TearDown]
        public void TearDown()
        {
            var amount = AsyncHelper.RunSync(() =>
                _cacheManager.ClearCacheByPatternAsync(CacheType, "*"));
            Console.WriteLine($"TearDown removed {amount} keys");
        }

        [Test]
        public async Task CacheList_caches_data_if_it_is_not_in_cache()
        {
            await _cacheManager.CacheListAsync(CacheType, _cacheKey, null, NewData);

            var listFromCache =
                await _cacheManager.GetListAsync(CacheType, _cacheKey, () => Task.FromResult(new List<string>()));
            listFromCache.Count.Should().Be(1);
            listFromCache[0].Should().Be(NewData);
        }

        [Test]
        public async Task CacheList_pushes_data_if_it_is_in_cache()
        {
            await _cacheManager.CacheListAsync(CacheType, _cacheKey, null, NewData);
            await _cacheManager.CacheListAsync(CacheType, _cacheKey, null, NewData);

            var listFromCache =
                await _cacheManager.GetListAsync(CacheType, _cacheKey, () => Task.FromResult(new List<string>()));
            listFromCache.Count.Should().Be(2);
            listFromCache[0].Should().Be(NewData);
            listFromCache[1].Should().Be(NewData);
        }

        [Test]
        public async Task GetList_creates_data_if_it_is_not_in_cache()
        {
            var createdList = await _cacheManager.GetListAsync(CacheType, _cacheKey,
                () => Task.FromResult(new List<string> { NewData, NewData }));
            createdList.Count.Should().Be(2);
            createdList[0].Should().Be(NewData);
            createdList[1].Should().Be(NewData);
        }

        [Test]
        public async Task ClearCache_clears_cache()
        {
            await _cacheManager.CacheAsync(CacheType, _cacheKey, null, NewData);
            await _cacheManager.ClearCacheAsync(CacheType, _cacheKey);

            var dataFromCache = await _cacheManager.GetAsync<string>(CacheType, _cacheKey);
            dataFromCache.Should().Be(default);
        }

        [Test]
        public async Task Set_expireIn_sets_expire()
        {
            var key = Guid.NewGuid();
            var cacheKey = RedisCacheManager.FormCacheKey(CacheType, key);

            await _cacheManager.SafeCacheAsync(CacheType, key, null, NewData);
            var database = _redisRepository.GetDb();
            var value = await database.StringGetWithExpiryAsync(cacheKey);
            var expiresIn = RedisCacheManager.CapExpiry(null);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);

            expiresIn = TimeSpan.FromSeconds(20);
            await _cacheManager.SetKeyExpirationInAsync(CacheType, key, expiresIn);
            value = await database.StringGetWithExpiryAsync(cacheKey);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);
        }

        [Test]
        public async Task Set_expireIn_changes_expire()
        {
            var key = Guid.NewGuid();
            var cacheKey = $"{CacheType}:{key}";

            var expiresIn = TimeSpan.FromSeconds(20);
            await _cacheManager.CacheAsync(CacheType, key, expiresIn, NewData);
            var database = _redisRepository.GetDb();
            var value = await database.StringGetWithExpiryAsync(cacheKey);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);

            expiresIn = TimeSpan.FromSeconds(30);
            await _cacheManager.SetKeyExpirationInAsync(CacheType, key, expiresIn);
            value = await database.StringGetWithExpiryAsync(cacheKey);
            value.Expiry.Should().BeCloseTo(expiresIn, 100);
        }

        [Test]
        public async Task ClearCacheByPattern_clear_right_keys()
        {
            var key11 = "1:11";
            var key12 = "1:12";
            var key21 = "1:21";
            var key22 = "1:22";

            await _cacheManager.CacheAsync(CacheType, key11, null, NewData);
            await _cacheManager.CacheAsync(CacheType, key12, null, NewData);
            await _cacheManager.CacheAsync(CacheType, key21, null, NewData);
            await _cacheManager.CacheAsync(CacheType, key22, null, NewData);

            var amount = await _cacheManager.ClearCacheByPatternAsync(CacheType, "1:2*");

            var key11Value = await _cacheManager.GetAsync<string>(CacheType, key11);
            var key12Value = await _cacheManager.GetAsync<string>(CacheType, key12);
            var key21Value = await _cacheManager.GetAsync<string>(CacheType, key21);
            var key22Value = await _cacheManager.GetAsync<string>(CacheType, key22);

            amount.Should().Be(2);
            key11Value.Should().Be(NewData);
            key12Value.Should().Be(NewData);
            key21Value.Should().Be(default);
            key22Value.Should().Be(default);
        }

        [Test]
        public async Task Can_ClearCacheByPattern_when_no_matches()
        {
            var amount = await _cacheManager.ClearCacheByPatternAsync(CacheType, $"{Guid.NewGuid()}*");
            amount.Should().Be(0);
        }

        [Test]
        public async Task Can_ClearCacheByPattern_when_lots_of_matches()
        {
            for (var i = 0; i < 2000; i++)
                await _cacheManager.CacheAsync(CacheType, $"pattern:{i % 2}:{i:D4}", null, NewData);
            var amount = await _cacheManager.ClearCacheByPatternAsync(CacheType, "pattern:0:*");
            amount.Should().Be(1000);
        }
    }
}
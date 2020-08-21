using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StackExchange.Redis;
using TGS.Infrastructures.Redis;

namespace TGS.Tests.Redis.IntegrationTests
{
    [TestFixture]
    [Category(Named.Integration)]
    public partial class RedisCacheManagerTests
    {
        private const string Field = "field";

        [Test]
        public async Task Can_cache_hash_data_when_no_exception()
        {
            await _cacheManager.CacheHashSetAsync(CacheType, _cacheKey, new HashEntry[]
            {
                new HashEntry(Field, NewData)
            }, null);

            var cacheKey = RedisCacheManager.FormCacheKey(CacheType, _cacheKey);
            var db = _redisRepository.GetDb();

            var value = await db.HashGetAsync(cacheKey, Field);

            value.IsNull.Should().BeFalse();
            value.Should().Be(NewData);
        }

        [Test]
        public async Task Can_get_cached_hash_data_when_no_exception()
        {
            var cacheKey = RedisCacheManager.FormCacheKey(CacheType, _cacheKey);
            var db = _redisRepository.GetDb();

            await db.HashSetAsync(cacheKey, new[]
            {
                new HashEntry(Field, NewData)
            });

            var values = await _cacheManager.GetHashSetAsync(CacheType, _cacheKey, new RedisValue[] {Field});

            values.Length.Should().Be(1);
            values[0].Should().Be(NewData);
        }
    }
}
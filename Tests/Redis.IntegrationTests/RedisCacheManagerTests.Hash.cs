using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using TGS.Tests.TestsCommon;

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
            await _cacheManager.SafeCacheHashSetAsync(CacheType, _cacheKey, new HashEntry[]
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

            var values = await _cacheManager.SafeGetHashSetAsync(CacheType, _cacheKey, new RedisValue[] {Field});

            values.Length.Should().Be(1);
            values[0].Should().Be(NewData);
        }

        [Test]
        public async Task Can_get_all_cached_hash_data_when_no_exception()
        {
            var cacheKey = RedisCacheManager.FormCacheKey(CacheType, _cacheKey);
            var db = _redisRepository.GetDb();

            var fields = Enumerable.Range(1, 5).Select(i => new HashEntry($"{Field}{i}", $"{NewData}_{i}")).ToArray();

            await db.HashSetAsync(cacheKey, fields);

            var values = await _cacheManager.SafeGetAllHashSetAsync(CacheType, _cacheKey);

            values.Length.Should().Be(fields.Length);
            foreach(var v in values)
            {
                fields.Should().Contain(v);
            }
        }
    }
}
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace TGS.Infrastructures.Redis.IntegrationTests
{
    [TestFixture]
    [Category(Named.Integration)]
    public partial class RedisCacheManagerTests
    {
        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_safely_get_data_when_no_exception(RedisConnectionType type)
        {
            var testQueries = Substitute.For<ITestQueries>();
            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManagerFactory(type.ToString()).SafeGetAsync(CacheType, _cacheKey, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(0);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_safely_get_data_when_redis_exception(RedisConnectionType type)
        {
            var diconnectedCacheManager = CreateDisconnectedCacheManager(type);

            var testQueries = Substitute.For<ITestQueries>();
            testQueries.GetDataAsync().Returns(Task.FromResult(NewData));

            var data = await diconnectedCacheManager.SafeGetAsync(
                CacheType, _cacheKey, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(1);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_safely_cache_data_when_no_exception(RedisConnectionType type)
        {
            var testQueries = Substitute.For<ITestQueries>();

            await _cacheManagerFactory(type.ToString()).SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManagerFactory(type.ToString()).SafeGetAsync(CacheType, _cacheKey, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(0);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_safely_try_to_cache_data_when_redis_exception(RedisConnectionType type)
        {
            var diconnectedCacheManager = CreateDisconnectedCacheManager(type);

            await diconnectedCacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            // checking that no exception was thrown
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_safely_get_and_cache_data_when_no_exception(RedisConnectionType type)
        {
            var testQueries = Substitute.For<ITestQueries>();
            testQueries.GetDataAsync().Returns(Task.FromResult(NewData));

            var data = await _cacheManagerFactory(type.ToString()).SafeGetAndCacheAsync(CacheType, _cacheKey, null, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(1);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_safely_get_and_cache_data_when_no_exception_and_precached(RedisConnectionType type)
        {
            var testQueries = Substitute.For<ITestQueries>();

            await _cacheManagerFactory(type.ToString()).CacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManagerFactory(type.ToString()).SafeGetAndCacheAsync(CacheType, _cacheKey, null, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(0);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_safely_get_and_cache_data_when_exception(RedisConnectionType type)
        {
            var diconnectedCacheManager = CreateDisconnectedCacheManager(type);

            var testQueries = Substitute.For<ITestQueries>();
            testQueries.GetDataAsync().Returns(Task.FromResult(NewData));

            var data = await diconnectedCacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(1);
        }

        public interface ITestQueries
        {
            Task<string> GetDataAsync();
        }

        private IRedisCacheManager CreateDisconnectedCacheManager(RedisConnectionType type)
        {
            return _cacheManagerFactory(type.ToString());
        }
    }
}
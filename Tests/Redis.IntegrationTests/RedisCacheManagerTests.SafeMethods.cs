using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Redis.IntegrationTests
{
    [TestFixture]
    [Category(Named.Integration)]
    public partial class RedisCacheManagerTests
    {
        [Test]
        public async Task Can_safely_get_data_when_no_exception()
        {
            var testQueries = Substitute.For<ITestQueries>();
            await _cacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManager.SafeGetAsync(CacheType, _cacheKey, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(0);
        }


        [Test]
        public async Task Can_safely_get_data_when_redis_exception()
        {
            var testQueries = Substitute.For<ITestQueries>();
            testQueries.GetDataAsync().Returns(Task.FromResult(NewData));

            var data = await _cacheManager.SafeGetAsync(
                CacheType, _cacheKey, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(1);
        }


        [Test]
        public async Task Can_safely_cache_data_when_no_exception()
        {
            var testQueries = Substitute.For<ITestQueries>();

            await _cacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManager.SafeGetAsync(CacheType, _cacheKey, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(0);
        }


        [Test]
        public async Task Can_safely_try_to_cache_data_when_redis_exception()
        {
            await _cacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            // checking that no exception was thrown
        }


        [Test]
        public async Task Can_safely_get_and_cache_data_when_no_exception()
        {
            var testQueries = Substitute.For<ITestQueries>();
            testQueries.GetDataAsync().Returns(Task.FromResult(NewData));

            var data = await _cacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(1);
        }


        [Test]
        public async Task Can_safely_get_and_cache_data_when_no_exception_and_precached()
        {
            var testQueries = Substitute.For<ITestQueries>();

            await _cacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(0);
        }


        [Test]
        public async Task Can_safely_get_and_cache_data_when_exception()
        {
            var testQueries = Substitute.For<ITestQueries>();
            testQueries.GetDataAsync().Returns(Task.FromResult(NewData));

            var data = await _cacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, () => testQueries.GetDataAsync());

            data.Should().Be(NewData);
            testQueries.Received(1);
        }

        public interface ITestQueries
        {
            Task<string> GetDataAsync();
        }
    }
}
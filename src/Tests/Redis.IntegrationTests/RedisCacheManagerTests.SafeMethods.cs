using System.Threading.Tasks;
using FluentAssertions;
using Moq;
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
        [Test]
        public async Task Can_safely_get_data_when_no_exception()
        {
            var testQueriesMock = new Mock<ITestQueries>();
            await _cacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            testQueriesMock.Setup(x => x.GetDataAsync())
                .Returns(Task.FromResult(NewData));

            var data = await _cacheManager.SafeGetAsync(CacheType, _cacheKey, testQueriesMock.Object.GetDataAsync);

            data.Should().Be(NewData);
            testQueriesMock.Verify(x => x.GetDataAsync(), Times.Never);
        }

        [Test]
        public async Task Can_safely_get_data_when_redis_exception()
        {
            var testQueriesMock = new Mock<ITestQueries>();
            testQueriesMock.Setup(x => x.GetDataAsync())
                .Returns(Task.FromResult(NewData));

            var redisMock = new Mock<IRedisRepository>();
            redisMock.Setup(x => x.GetDb())
                .Throws(new RedisException("fail"));

            var data = await _cacheManager.SafeGetAsync(
                CacheType, _cacheKey, testQueriesMock.Object.GetDataAsync);

            data.Should().Be(NewData);
            testQueriesMock.Verify(x => x.GetDataAsync(), Times.Once);
        }


        [Test]
        public async Task Can_safely_cache_data_when_no_exception()
        {
            var testQueriesMock = new Mock<ITestQueries>();
            testQueriesMock.Setup(x => x.GetDataAsync())
                .Returns(Task.FromResult(NewData));

            await _cacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManager.SafeGetAsync(CacheType, _cacheKey, testQueriesMock.Object.GetDataAsync);

            data.Should().Be(NewData);
            testQueriesMock.Verify(x => x.GetDataAsync(), Times.Never);
        }


        [Test]
        public async Task Can_safely_try_to_cache_data_when_redis_exception()
        {            
            var testQueriesMock = new Mock<ITestQueries>();
            testQueriesMock.Setup(x => x.GetDataAsync())
                .Returns(Task.FromResult(NewData));

            var redisMock = new Mock<IRedisRepository>();
            redisMock.Setup(x => x.GetDb())
                .Throws(new RedisException("fail"));

            var data = await _cacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, testQueriesMock.Object.GetDataAsync);

            data.Should().Be(NewData);
            testQueriesMock.Verify(x => x.GetDataAsync(), Times.Once);
        }


        [Test]
        public async Task Can_safely_get_and_cache_data_when_no_exception()
        {
            var testQueriesMock = new Mock<ITestQueries>();
            testQueriesMock.Setup(x => x.GetDataAsync())
                .Returns(Task.FromResult(NewData));

            var data = await _cacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, testQueriesMock.Object.GetDataAsync);

            data.Should().Be(NewData);
            testQueriesMock.Verify(x => x.GetDataAsync(), Times.Once);
        }

        [Test]
        public async Task Can_safely_get_and_cache_data_when_no_exception_and_precached()
        {
            var testQueriesMock = new Mock<ITestQueries>();
            testQueriesMock.Setup(x => x.GetDataAsync())
                .Returns(Task.FromResult(NewData));

            await _cacheManager.SafeCacheAsync(CacheType, _cacheKey, null, NewData);

            var data = await _cacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, testQueriesMock.Object.GetDataAsync);

            data.Should().Be(NewData);
            testQueriesMock.Verify(x => x.GetDataAsync(), Times.Never);
        }

        [Test]
        public async Task Can_safely_get_and_cache_data_when_exception()
        {
            var testQueriesMock = new Mock<ITestQueries>();
            testQueriesMock.Setup(x => x.GetDataAsync())
                .Returns(Task.FromResult(NewData));

            var redisMock = new Mock<IRedisRepository>();
            redisMock.Setup(x => x.GetDb())
                .Throws(new RedisException("fail"));

            var data = await _cacheManager.SafeGetAndCacheAsync(CacheType, _cacheKey, null, testQueriesMock.Object.GetDataAsync);

            data.Should().Be(NewData);
            testQueriesMock.Verify(x => x.GetDataAsync(), Times.Once);
        }

        public interface ITestQueries
        {
            Task<string> GetDataAsync();
        }
    }
}
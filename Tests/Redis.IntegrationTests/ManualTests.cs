using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Redis.IntegrationTests
{
    [TestFixture, Parallelizable]
    [Category(Named.Manual)]
    [Explicit(reason: Named.NotToBeContinuouslyTested)]
    public class ManualTests
    {
        private const string CacheType = "ManualTests";

        private IRedisRepository _redisRepository;
        private static IRedisCacheManager _cacheManager;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var provider = new ServiceCollection()
                .AddConfiguration()
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .BuildServiceProvider();

            _redisRepository = provider.GetService<IRedisRepository>();
            _cacheManager = provider.GetService<IRedisCacheManager>();
        }

        [TearDown]
        public void TearDown()
        {
            var amount = AsyncHelper.RunSync(() =>
                _cacheManager.ClearCacheByPatternAsync(CacheType, "*"));
            Console.WriteLine($"TearDown removed {amount} keys");
        }

        [TestCase(100000, 2000)]
        [TestCase(1000000, 20000)]
        [TestCase(1000000, 10000)]
        [TestCase(1000000, 100000)]
        [TestCase(5000000, 1000000)]
        public async Task Remove_keys_by_pattern(int amount, int toRemove)
        {
            var divider = amount / toRemove;

            Console.WriteLine("Starting to create keys");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < amount; i++)
            {
                var key = $"{i:D10}:{i % divider}:{i % 200000}";
                await _cacheManager.CacheAsync(CacheType, key, null, i);
            }
            stopwatch.Stop();
            Console.WriteLine($"Created {amount} keys in {stopwatch.Elapsed}");

            Console.WriteLine("Starting to remove keys");
            stopwatch.Reset();
            stopwatch.Start();
            var count = await _cacheManager.ClearCacheByPatternAsync(CacheType, "*:0:*");
            stopwatch.Stop();
            Console.WriteLine($"Removed {count} keys in {stopwatch.Elapsed}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace TGS.Infrastructures.Redis.IntegrationTests
{
    [TestFixture]
    [Category(Named.Integration)]
    public class RedisLockTests
    {
        private IRedisLock _redisLock;
        private static object Locker = new object();

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var provider = new ServiceCollection()
                .AddConfiguration()
                .AddRedis()
                .BuildServiceProvider();

            _redisLock = provider.GetService<IRedisLock>();
        }


        [Test]
        public async Task TestSingleLock()
        {
            var resource = $"testredislock:{Guid.NewGuid()}";
            IRedisLockSummary firstLock = null;

            await _redisLock.ExecuteLockAsync(resource, TimeSpan.FromSeconds(10),
                 c =>
                {
                    firstLock = c;
                    firstLock.IsAcquired.Should().BeTrue();
                    firstLock.IsReleased.Should().BeFalse();
                    return Task.FromResult(true);
                }, _ => true);

            firstLock.IsReleased.Should().BeTrue();
        }


        [Test]
        public async Task TestOverlappingLocks()
        {
            var resource = $"testredislock:{Guid.NewGuid()}";

            IRedisLockSummary firstLock = null;
            await _redisLock.ExecuteLockAsync(resource, TimeSpan.FromSeconds(10),
                async c =>
                {
                    firstLock = c;
                    firstLock.IsAcquired.Should().BeTrue();
                    IRedisLockSummary secondLock = null;
                    await _redisLock.ExecuteLockAsync(resource, TimeSpan.FromSeconds(10),
                        cc => Task.FromResult(true), cc =>
                        {
                            secondLock = cc;
                            secondLock.IsAcquired.Should().BeFalse();
                            firstLock.IsReleased.Should().BeFalse();
                            return true;
                        });
                    secondLock.IsReleased.Should().BeFalse();
                    return true;
                }, _ => true);

            firstLock.IsReleased.Should().BeTrue();
        }


        [Test]
        public async Task TestSequentialLocks()
        {
            var resource = $"testredislock:{Guid.NewGuid()}";

            IRedisLockSummary firstLock = null;

            await _redisLock.ExecuteLockAsync(resource, TimeSpan.FromSeconds(10),
                c =>
                {
                    firstLock = c;
                    firstLock.IsAcquired.Should().BeTrue();
                    firstLock.IsReleased.Should().BeFalse();
                    return Task.FromResult(true);
                }, _ => true);

            firstLock.IsReleased.Should().BeTrue();

            await _redisLock.ExecuteLockAsync(resource, TimeSpan.FromSeconds(10),
                c =>
                {
                    firstLock = c;
                    firstLock.IsAcquired.Should().BeTrue();
                    firstLock.IsReleased.Should().BeFalse();
                    return Task.FromResult(true);
                }, _ => true);
            firstLock.IsReleased.Should().BeTrue();
        }


        [Test]
        public async Task TestLockReleasedAfterTimeout()
        {
            var resource = $"testredislock:{Guid.NewGuid()}";

            var expiredTime = TimeSpan.FromSeconds(3);
            IRedisLockSummary firstLock = null;

            await _redisLock.ExecuteLockAsync(resource, TimeSpan.FromSeconds(3),
                async c =>
                {
                    firstLock = c;
                    firstLock.IsAcquired.Should().BeTrue();
                    var waitTime = expiredTime.Add(TimeSpan.FromSeconds(1));
                    await Task.Delay(waitTime);
                    IRedisLockSummary secondLock = null;
                    await _redisLock.ExecuteLockAsync(resource, TimeSpan.FromSeconds(10),
                        cc =>
                        {
                            secondLock = cc;
                            secondLock.IsAcquired.Should().BeTrue();
                            return Task.FromResult(true);
                        }, cc => true);
                    secondLock.IsReleased.Should().BeTrue();
                    return true;
                }, _ => true);

            firstLock.IsReleased.Should().BeFalse();
        }

        [TestCase(100), Timeout(30000)]
        public async Task TestBlockingConcurrentLocks(int totalTask)
        {
            var resource = $"testredislock:{Guid.NewGuid()}";

            var expiredTime = TimeSpan.FromSeconds(60);
            var lockCount = 0;
            var faillockCount = 0;

            var blocker = new AutoResetEvent(false);

            var list = new List<Task>();

            for (var i = 0; i < totalTask; i++)
            {
                list.Add(LockAsync());
            }

            await Task.WhenAll(list);

            lockCount.Should().Be(1);
            faillockCount.Should().Be(list.Count - 1);

            Task LockAsync()
            {
                return _redisLock.ExecuteLockAsync(resource, expiredTime,
                    c =>
                    {
                        lockCount++;

                        Console.WriteLine($" {resource} got lock at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");
                        //keep lock
                        blocker.WaitOne();
                        return Task.FromResult(true);
                    }, _ =>
                    {
                        lock (Locker)
                        {
                            faillockCount++;
                            if (faillockCount + lockCount == totalTask)
                            {
                                Console.WriteLine($"failed lock counts: {faillockCount} at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");
                                blocker.Set();
                            }
                        }
                        return true;
                    });
            }
        }

        [Test]
        public async Task TestWaitLocks()
        {
            var resource = ":::testredislock:::";

            var expiredTime = TimeSpan.FromSeconds(60);
            var lockCount = 0;
            var faillockCount = 0;

            var blocker = new ManualResetEvent(false);

            var list = new List<Task>
            {
                LockAsync(),
                LockAsync(),
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(6));

                    // release block after time
                    blocker.Set();
                    Console.WriteLine($"{resource} release lock at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");
                })
            };

            await Task.WhenAll(list);

            lockCount.Should().Be(2);
            faillockCount.Should().BeGreaterOrEqualTo(2);

            Task LockAsync()
            {
                return _redisLock.ExecuteLockAsync(resource, expiredTime,
                    c =>
                    {
                        lockCount++;

                        Console.WriteLine($" {resource} got lock at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");
                        //keep lock
                        blocker.WaitOne();
                        return Task.FromResult(true);
                    }, _ =>
                    {
                        faillockCount++;

                        Console.WriteLine($"failed lock counts( {faillockCount}), retring... at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");

                        return true;
                    }, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3));
            }
        }

        [Test]
        public async Task TestWaitLocksNoReturn()
        {
            var resource = ":::testredislock:::";

            var expiredTime = TimeSpan.FromSeconds(60);
            var lockCount = 0;
            var faillockCount = 0;

            var blocker = new ManualResetEvent(false);

            var list = new List<Task>
            {
                LockAsync(),
                LockAsync(),
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(6));

                    // release block after time
                    blocker.Set();
                    Console.WriteLine($"{resource} release lock at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");
                })
            };

            await Task.WhenAll(list);

            lockCount.Should().Be(2);
            faillockCount.Should().BeGreaterOrEqualTo(2);

            Task LockAsync()
            {
                return _redisLock.ExecuteLockAsync(resource, expiredTime,
                     () =>
                    {
                        lockCount++;

                        Console.WriteLine($" {resource} got lock at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");
                        //keep lock
                        blocker.WaitOne();
                        return Task.CompletedTask;
                    },
                     () =>
                 {
                     faillockCount++;

                     Console.WriteLine($"failed lock counts( {faillockCount}), retring... at {DateTimeOffset.UtcNow.ToString("hh:mm:ss.fff")} ");
                     return Task.CompletedTask;
                 }, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3));
            }
        }
    }
}
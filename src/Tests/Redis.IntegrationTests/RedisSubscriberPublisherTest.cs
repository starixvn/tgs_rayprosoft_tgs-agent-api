using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Redis.IntegrationTests
{
    [TestFixture, Parallelizable]
    [Category(Named.Integration)]
    public class RedisSubscriberPublisherTest
    {
        private IRedisPublisher _redisPublisher;
        private IRedisSubscriber _redisSubscriber;

        [SetUp]
        public void SetUp()
        {
            var provider = new ServiceCollection()
                .AddConfiguration()
                .AddTgsLog()
                .AddLogging()
                .AddRedis()                
                .BuildServiceProvider();

            _redisPublisher = provider.GetService<IRedisPublisher>();
            _redisSubscriber = provider.GetService<IRedisSubscriber>();
        }

        [TearDown]
        public void TearDown()
        {
            AsyncHelper.RunSync(() =>
                    _redisSubscriber.UnsubscribeAsync<TestValue>());
        }


        [Test]
        public async Task Can_Subscribe_When_Execute_Publish_By_Object()
        {
            var value = new TestValue { Id = 2, Name = "TGS" };
            var result = new TestValue();
            var tcs = new TaskCompletionSource<bool>();
            await _redisSubscriber.SubscribeAsync<TestValue>(message =>
            {
                result = message;
                tcs.SetResult(true);
            });

            await _redisPublisher.PublishAsync(value);
            await tcs.Task;

            result.Id.Should().Be(value.Id);
            result.Name.Should().Be(value.Name);
        }


        [Test]
        public async Task Can_Subscribe_And_Unsubscribe()
        {
            var value = new TestValue { Id = 12, Name = "ValueToGet" };
            var result = new TestValue();

            await _redisSubscriber.SubscribeAsync<TestValue>(message =>
            {
                Console.WriteLine($"Processing {message.Id}:{message.Name}");
                result = message;
            });

            await _redisPublisher.PublishAsync(value);
            await Task.Delay(300);
            await _redisSubscriber.UnsubscribeAsync<TestValue>();
            await Task.Delay(300);
            await _redisPublisher.PublishAsync(new TestValue { Id = 23, Name = "NotReceivedValue" });
            await Task.Delay(300);

            result.Id.Should().Be(value.Id);
            result.Name.Should().Be(value.Name);
        }


        [Test]
        public async Task Exception_does_not_break_subscription()
        {
            var result = new TestValue();
            var messagesProcessed = 0;

            await _redisSubscriber.SubscribeAsync<TestValue>(message =>
            {
                Console.WriteLine($"Processing {message.Id}:{message.Name}");

                messagesProcessed++;
                if (message.Name == "Error")
                {
                    Console.WriteLine("Throwing exception");
                    throw new ArgumentException("Should be thrown to check that subscription will still work");
                }

                Console.WriteLine($"Processed {message.Id}:{message.Name}");
                result = message;
            });

            var item = new TestValue
            {
                Id = 13,
                Name = "Correct"
            };
            await _redisPublisher.PublishAsync(new TestValue { Id = 7, Name = "Error" });
            await Task.Delay(300); // to ensure that first message will be processed first
            await _redisPublisher.PublishAsync(item);
            await Task.Delay(300);

            messagesProcessed.Should().Be(2);
            result.Id.Should().Be(item.Id);
            result.Name.Should().Be(item.Name);
        }


        [Test]
        public async Task Exception_does_not_break_subscription_using_real_async()
        {
           var result = new TestValue();
            var messagesProcessed = 0;

            await _redisSubscriber.SubscribeAsync<TestValue>(async message =>
            {
                Console.WriteLine($"Processing {message.Id}:{message.Name}");

                messagesProcessed++;
                if (message.Name == "Error")
                {
                    Console.WriteLine("Throwing exception");
                    await ThrowExceptionAsync();
                }

                Console.WriteLine($"Processed {message.Id}:{message.Name}");
                result = message;
            });

            var item = new TestValue
            {
                Id = 13,
                Name = "Correct"
            };
            await _redisPublisher.PublishAsync(new TestValue { Id = 7, Name = "Error" });
            await Task.Delay(300); // to ensure that first message will be processed first
            await _redisPublisher.PublishAsync(item);
            await Task.Delay(300);

            messagesProcessed.Should().Be(2);
            result.Id.Should().Be(item.Id);
            result.Name.Should().Be(item.Name);
        }

        private static Task ThrowExceptionAsync() => 
            Task.Run(() => new ArgumentException("Some test exception"));

        public class TestValue
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
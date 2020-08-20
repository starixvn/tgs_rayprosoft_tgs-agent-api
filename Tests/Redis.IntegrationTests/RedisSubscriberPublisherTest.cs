using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace TGS.Infrastructures.Redis.IntegrationTests
{
    [TestFixture, Parallelizable]
    [Category(Named.Integration)]
    public class RedisSubscriberPublisherTest
    {
        private Func<string, IRedisPublisher> _redisPublisherFactory;
        private Func<string, IRedisSubscriber> _redisSubscriberFactory;

        [SetUp]
        public void SetUp()
        {
            var provider = new ServiceCollection()
                .AddRedisServices()
                .BuildServiceProvider();

            _redisPublisherFactory = provider.GetService<Func<string, IRedisPublisher>>();
            _redisSubscriberFactory = provider.GetService<Func<string, IRedisSubscriber>>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var type in Enum.GetNames(typeof(RedisConnectionType)))
            {
                AsyncHelper.RunSync(() =>
                    _redisSubscriberFactory(type).UnsubscribeAsync<TestValue>());
            }
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_Subscribe_When_Execute_Publish_By_Object(RedisConnectionType type)
        {
            var value = new TestValue { Id = 2, Name = "TGS" };
            var result = new TestValue();
            var tcs = new TaskCompletionSource<bool>();
            await _redisSubscriberFactory(type.ToString()).SubscribeAsync<TestValue>(message =>
            {
                result = message;
                tcs.SetResult(true);
            });

            await _redisPublisherFactory(type.ToString()).PublishAsync(value);
            await tcs.Task;

            result.Id.Should().Be(value.Id);
            result.Name.Should().Be(value.Name);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Can_Subscribe_And_Unsubscribe(RedisConnectionType type)
        {
            var value = new TestValue { Id = 12, Name = "ValueToGet" };
            var result = new TestValue();

            await _redisSubscriberFactory(type.ToString()).SubscribeAsync<TestValue>(message =>
            {
                Console.WriteLine($"Processing {message.Id}:{message.Name}");
                result = message;
            });

            await _redisPublisherFactory(type.ToString()).PublishAsync(value);
            await Task.Delay(300);
            await _redisSubscriberFactory(type.ToString()).UnsubscribeAsync<TestValue>();
            await Task.Delay(300);
            await _redisPublisherFactory(type.ToString()).PublishAsync(new TestValue { Id = 23, Name = "NotReceivedValue" });
            await Task.Delay(300);

            result.Id.Should().Be(value.Id);
            result.Name.Should().Be(value.Name);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Exception_does_not_break_subscription(RedisConnectionType type)
        {
            var result = new TestValue();
            var messagesProcessed = 0;

            await _redisSubscriberFactory(type.ToString()).SubscribeAsync<TestValue>(message =>
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
            await _redisPublisherFactory(type.ToString()).PublishAsync(new TestValue { Id = 7, Name = "Error" });
            await Task.Delay(300); // to ensure that first message will be processed first
            await _redisPublisherFactory(type.ToString()).PublishAsync(item);
            await Task.Delay(300);

            messagesProcessed.Should().Be(2);
            result.Id.Should().Be(item.Id);
            result.Name.Should().Be(item.Name);
        }

        [TestCase(RedisConnectionType.Core)]
        [TestCase(RedisConnectionType.SignalR)]
        public async Task Exception_does_not_break_subscription_using_real_async(RedisConnectionType type)
        {
            var result = new TestValue();
            var messagesProcessed = 0;

            await _redisSubscriberFactory(type.ToString()).SubscribeAsync<TestValue>(async message =>
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
            await _redisPublisherFactory(type.ToString()).PublishAsync(new TestValue { Id = 7, Name = "Error" });
            await Task.Delay(300); // to ensure that first message will be processed first
            await _redisPublisherFactory(type.ToString()).PublishAsync(item);
            await Task.Delay(300);

            messagesProcessed.Should().Be(2);
            result.Id.Should().Be(item.Id);
            result.Name.Should().Be(item.Name);
        }

        private static Task ThrowExceptionAsync()
        {
            throw new ArgumentException("Some test exception");
        }

        public class TestValue
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TGS.Core.Infrastructures.Nats;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Nats.IntegrationTests
{
    [TestFixture]
    [Category(Named.Integration), Ignore("Nats not work")]
    public class NatsManagerTests
    {
        private const string Topic = "NatsTopicTest";
        private ServiceProvider _serviceProvider;
        [SetUp]
        public void Init()
        {
            var serviceCollection = new ServiceCollection()
                .AddConfiguration();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceProvider = serviceCollection.AddNats(_serviceProvider.GetService<IConfiguration>())
                .BuildServiceProvider();
        }

        [Test, Timeout(10000)]
        public async Task CanPublishAndSubscribeMessages()
        {
            var ev = new AutoResetEvent(false);
            var natsManager = _serviceProvider.GetService<INatsManager>();
            natsManager?.SubscribeAsync<string>(Topic, message =>
            {
                message.Subject.Should().Be(Topic);
                message.DataObject.Should().Be("Value");
                ev.Set();
            });
            await natsManager.PublishAsync(Topic, "Value");
            ev.WaitOne();
            natsManager.Dispose();
        }

        [Test, Timeout(10000)]
        public async Task CanPublishAndSubscribeObjectMessages()
        {
            var ev = new AutoResetEvent(false);
            var testObj = new TestObject
            {
                TestString = "testString",
                TestInt = 12345,
                TestClass = new InternalTestClass { InternalTestString = "internalTestString" }
            };
            var natsManager = _serviceProvider.GetService<INatsManager>();
            natsManager.SubscribeAsync<TestObject>(Topic, message =>
            {
                message.Subject.Should().Be(Topic);
                message.DataObject.Should().BeEquivalentTo(testObj);
                ev.Set();
            });
            await natsManager.PublishAsync(Topic, testObj);
            ev.WaitOne();
            natsManager.Dispose();
        }

        [MessagePackObject(true)]
        public class TestObject
        {
            public string TestString { set; get; }
            public int TestInt { set; get; }
            public InternalTestClass TestClass { set; get; }
        }

        [MessagePackObject(true)]
        public class InternalTestClass
        {
            public string InternalTestString { set; get; }
        }
    }
}
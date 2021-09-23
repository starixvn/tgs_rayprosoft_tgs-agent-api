using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Tgs.Logging;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Nats.IntegrationTests
{
    [TestFixture]
    [Category(Named.Integration)]
    public class NatsManagerTests
    {
        private const string Topic = "NatsTopicTest";

        [Test, Timeout(10000)]
        public async Task CanPublishAndSubscribeMessages()
        {
            var ev = new AutoResetEvent(false);
            var serviceProvider = CreateServiceProvider();
            var natsManager = serviceProvider.GetService<INatsManager>();
            natsManager?.SubscribeAsync<string>(Topic, message =>
            {
                message.Subject.Should().Be(Topic);
                message.DataObject.Should().Be("Value");
                ev.Set();
            });
            await natsManager.PublishAsync(Topic, "Value");
            ev.WaitOne();
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
            var serviceProvider = CreateServiceProvider();
            var natsManager = serviceProvider.GetService<INatsManager>();
            natsManager.SubscribeAsync<TestObject>(Topic, message =>
            {
                message.Subject.Should().Be(Topic);
                message.DataObject.Should().BeEquivalentTo(testObj);
                ev.Set();
            });
            await natsManager.PublishAsync(Topic, testObj);
            ev.WaitOne();
        }

        private ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

#if DEBUG
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            serviceCollection.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
#else
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.IntTest.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);
#endif

            return serviceCollection
                .AddNats(configuration)
                .BuildServiceProvider();
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

using System;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TGS.Core.Models;

namespace Tgs.UnitTests.TgsApi.Startups
{
    public class StartupTestsBase
    {
        protected static void ResolveRegistrationsAndControllers<TStartup>(Type[] controllerTypes)
        {
            var natsSettingMock = new Mock<IConfigurationSection>();
            natsSettingMock.Setup(x => x["Urls"]).Returns("nats://localhost:4222");
            natsSettingMock.Setup(x => x["ConnectTimeoutMs"]).Returns("2000");
            natsSettingMock.Setup(x => x["PingInterval"]).Returns("5000");
            natsSettingMock.Setup(x => x["PingMaxOutStanding"]).Returns("3");
            natsSettingMock.Setup(x => x["SubscriptionBatchSize"]).Returns("128");

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection("NatsSetting")).Returns(natsSettingMock.Object);
            configurationMock.Setup(x => x.GetSection(Consts.EnvironmentVariable.GameProviderCode))
                .Returns(new Mock<IConfigurationSection>().Object);

            var envMock = new Mock<IWebHostEnvironment>();

            IServiceCollection services = new ServiceCollection();

            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            // dynamic create TStartup object and call ConfigureServices method
            var newExp = Expression.New(
                typeof(TStartup).GetConstructor(new[] { typeof(IConfiguration), typeof(IWebHostEnvironment) }),
                Expression.Constant(configurationMock.Object), Expression.Constant(envMock.Object));

            var callExp = Expression.Call(
                newExp,
                typeof(TStartup).GetMethod("ConfigureServices", new Type[] { services.GetType() }), 
                Expression.Constant(services));
            var func = Expression.Lambda<Action>(callExp).Compile();
            func();

            var sp = services.BuildServiceProvider();

            foreach (var type in controllerTypes)
            { 
                var controller = sp.GetService(type);
                controller.Should().NotBeNull();
            }
        }
    }
}

// ReSharper disable AssignNullToNotNullAttribute

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TGS.Apis.TgsApi.Commons.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.TgsApi.Commons;

namespace TGS.UnitTests.Startups.TgsApi
{
    public class TgsApiStartupTestsBase<TStartup> : StartupTestsBase where TStartup : BaseStartup
    {
        private readonly string _gameProviderCode;

        protected TgsApiStartupTestsBase(string gameProviderCode)
        {
            _gameProviderCode = gameProviderCode;
        }

        protected void ResolveAllRegistrationsWithAdditionalTypes(Type[] resolveTypes, Type[] ignoreTypes = null, string namespacePrefix = "TGS")
        {
            ignoreTypes ??= Array.Empty<Type>();

            var additionalControllerTypes = new[] {typeof(PluggableController)};
            var types = resolveTypes.Concat(additionalControllerTypes)
                .Except(ignoreTypes);

            base.ResolveAllRegistrations(types, namespacePrefix);
        }

        public override void InitialStartupConfigureServices(IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment env)
        {
            // dynamic create TStartup object and call ConfigureServices method
            var constructor =
                typeof(TStartup).GetConstructor(new[] {typeof(IConfiguration), typeof(IWebHostEnvironment)});
            var newExp = Expression.New(constructor, Expression.Constant(configuration), Expression.Constant(env));

            var method = typeof(TStartup).GetMethod("ConfigureServices", new[] {services.GetType()});
            var callExp = Expression.Call(newExp, method, Expression.Constant(services));
            var func = Expression.Lambda<Action>(callExp).Compile();
            func();
        }

        protected override IConfiguration MockConfiguration()
        {
            var config = new Dictionary<string, string>
            {
                {"ConnectionStrings:Tgs", "server=localhost;database=Tgs;user=sa;password=xxx"},
                {"ConnectionStrings:Bet", "server=localhost;database=Bet;user=sa;password=xxx"},
                {"ConnectionStrings:Log", "server=localhost;database=Tgs;user=sa;password=xxx"},
                {"ConnectionStrings:RedisConfigurationString", "localhost,abortConnect=false,connectTimeout=3000"},
                {"NatsSetting:Urls", "nats://localhost:4222"},
                {"NatsSetting:ConnectTimeoutMs", "2000"},
                {"NatsSetting:PingInterval", "5000"},
                {"NatsSetting:PingMaxOutStanding", "3"},
                {"NatsSetting:SubscriptionBatchSize", "128"},
                {$"FeatureManagement:{_gameProviderCode}", "true"},
                {"Logging:LogLevel:Default", "Information"},
                {"Logging:LogLevel:Microsoft", "Warning"},
                {"Logging:LogLevel:Microsoft.Hosting.Lifetime", "Information"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            return configuration;
        }
    }
}
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests
{
    public abstract class SpecFlowIntegrationTestBase : SpecFlowUnitTestBase
    {
        protected IServiceProvider ServiceProvider { get; }

        protected SpecFlowIntegrationTestBase(ScenarioContext context)
            : base(context)
        {
            var services = (IServiceCollection) new ServiceCollection();
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            AddRequiredServices(services, configuration);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected abstract void AddRequiredServices(IServiceCollection services, IConfiguration configuration);

        protected T GetDb<T>() where T : IBaseRepository
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}

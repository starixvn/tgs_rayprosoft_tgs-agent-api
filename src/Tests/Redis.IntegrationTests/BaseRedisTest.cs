using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Redis.IntegrationTests
{
    public class BaseRedisTest
    {
        protected ServiceProvider GetServiceProvider()
        {
            var sc = new ServiceCollection();
#if DEBUG
            sc.AddConfiguration();

#else
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile($"appsettings.IntTest.json", optional: true)
               .AddEnvironmentVariables()
               .Build();

            sc.AddSingleton<IConfiguration>(configuration);
#endif

            return sc
               .AddWebApiServices()
               .AddTgsLog()
               .AddRedis()
               .BuildServiceProvider();
        }
    } 
}

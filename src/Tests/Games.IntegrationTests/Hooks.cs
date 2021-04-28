using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using TechTalk.SpecFlow;
using TGS.Apis.OperatorApi;
using TGS.Apis.TgsApi.GameProviders.Habanero;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.SBOBET;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;

namespace TGS.Tests.Games.IntegrationTests
{
    [Binding]
    public class Hooks
    {
        private static IHost _OpApiHost;

        private static IHost _TgsApiHost;

        public static HttpClient OpApiClient { get; private set; }

        public static HttpClient TgsApiClient { get; private set; }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            if (featureContext.FeatureInfo.Tags.Contains("Habanero"))
            {
                CreateHabaneroEnv();
            }

            if (featureContext.FeatureInfo.Tags.Contains("PragmaticPlay"))
            {
                CreatePragmaticPlayEnv();
            }

            if (featureContext.FeatureInfo.Tags.Contains("SBOBET"))
            {
                CreateSbobetEnv();
            }
        }

        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext)
        {
            Task.WhenAll(_OpApiHost.StopAsync(), _TgsApiHost.StopAsync());
        }

        private static void CreateSbobetEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.SBOBET);

            var fakeAnalyzer = new Mock<IRequestAnalyzer>();
            fakeAnalyzer.Setup(x => x.GetIp(It.IsAny<HttpContext>())).Returns("127.0.0.1");

            var currentDictionary = Directory.GetCurrentDirectory();

            _OpApiHost = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Path.GetFullPath(Path.Combine(currentDictionary,
                        "../../../../../Apis/OperatorApi")));
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseStartup<Startup>()
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            _OpApiHost.StartAsync();
            OpApiClient = _OpApiHost.GetTestClient();

            _TgsApiHost = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder =>
                {
                    builder
                        .SetBasePath(Path.GetFullPath(Path.Combine(currentDictionary,
                            "../../../../../Apis/TgsApi")))
                        .AddJsonFile("GameProviders/SBOBET/sbobet.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseStartup<SBOBETStartup>()
                        .UseKestrel(options => options.AddServerHeader = false)
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            _TgsApiHost.StartAsync();
            TgsApiClient = _TgsApiHost.GetTestClient();
        }

        private static void CreatePragmaticPlayEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.PragmaticPlay);

            var fakeAnalyzer = new Mock<IRequestAnalyzer>();
            fakeAnalyzer.Setup(x => x.GetIp(It.IsAny<HttpContext>())).Returns("127.0.0.1");

            var currentDictionary = Directory.GetCurrentDirectory();

            _OpApiHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    var path = Path.Combine(currentDictionary, "../../../../../Apis/OperatorApi");
                    builder.SetBasePath(Path.GetFullPath(path));
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>()
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            _OpApiHost.StartAsync();
            OpApiClient = _OpApiHost.GetTestClient();

            _TgsApiHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    var path = Path.Combine(currentDictionary, "../../../../../Apis/TgsApi");

                    builder.SetBasePath(Path.GetFullPath(path))
                        .AddJsonFile("GameProviders/PragmaticPlay/pragmaticplay.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<PragmaticPlayStartup>()
                        .UseKestrel(o => o.AddServerHeader = false)
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            _TgsApiHost.StartAsync();
            TgsApiClient = _TgsApiHost.GetTestClient();
        }

        private static void CreateHabaneroEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.Habanero);

            var fakeAnalyzer = new Mock<IRequestAnalyzer>();
            fakeAnalyzer.Setup(x => x.GetIp(It.IsAny<HttpContext>())).Returns("127.0.0.1");

            var currentDictionary = Directory.GetCurrentDirectory();

            _OpApiHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    var path = Path.Combine(currentDictionary, "../../../../../Apis/OperatorApi");
                    builder.SetBasePath(Path.GetFullPath(path));
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>()
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            _OpApiHost.StartAsync();
            OpApiClient = _OpApiHost.GetTestClient();

            _TgsApiHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    var path = Path.Combine(currentDictionary, "../../../../../Apis/TgsApi");

                    builder.SetBasePath(Path.GetFullPath(path))
                        .AddJsonFile("GameProviders/Habanero/habanero.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<HabaneroStartup>()
                        .UseKestrel(o => o.AddServerHeader = false)
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            _TgsApiHost.StartAsync();
            TgsApiClient = _TgsApiHost.GetTestClient();
        }
    }
}

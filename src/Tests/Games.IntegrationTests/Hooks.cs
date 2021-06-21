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
using TGS.Apis.TgsApi.GameProviders.AsiaGaming;
using TGS.Apis.TgsApi.GameProviders.BigGaming;
using TGS.Apis.TgsApi.GameProviders.Habanero;
using TGS.Apis.TgsApi.GameProviders.IMESports;
using TGS.Apis.TgsApi.GameProviders.JDB;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.Saba;
using TGS.Apis.TgsApi.GameProviders.SBOBET;
using TGS.Apis.TgsApi.GameProviders.SexyGaming;
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

            if (featureContext.FeatureInfo.Tags.Contains("AsiaGaming"))
            {
                CreateAsiaGamingEnv();
            }

            if (featureContext.FeatureInfo.Tags.Contains("Saba"))
            {
                CreateSabaEnv();
            }

            if (featureContext.FeatureInfo.Tags.Contains("IMESports"))
            {
                CreateIMESportsEnv();
            }

            if (featureContext.FeatureInfo.Tags.Contains("BigGaming"))
            {
                CreateBigGamingEnv();
            }
            
            if (featureContext.FeatureInfo.Tags.Contains("JDB"))
            {
                CreateJDBEnv();
            }


            if (featureContext.FeatureInfo.Tags.Contains("SexyGaming"))
            {
                CreateSexyGamingEnv();
            }
        }

        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext) =>
            Task.WhenAll(_OpApiHost.StopAsync(), _TgsApiHost.StopAsync());

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

        private static void CreateAsiaGamingEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.AsiaGaming);

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
                        .AddJsonFile("GameProviders/AsiaGaming/asiagaming.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<AsiaGamingStartup>()
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

        private static void CreateSabaEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.Saba);

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
                        .AddJsonFile("GameProviders/Saba/saba.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<SabaStartup>()
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

        private static void CreateIMESportsEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.IMESports);

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
                        .AddJsonFile("GameProviders/IMESports/imesports.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<IMESportsStartup>()
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

        private static void CreateBigGamingEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.BigGaming);

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
                        .AddJsonFile("GameProviders/BigGaming/biggaming.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<BigGamingStartup>()
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


        private static void CreateSexyGamingEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.SexyGaming);

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
                        .AddJsonFile("GameProviders/SexyGaming/sexygaming.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<SexyGamingStartup>()
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
        
        private static void CreateJDBEnv()
        {
            Environment.SetEnvironmentVariable("GameProviderCode", Consts.GameProvider.JDB);

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
                        .AddJsonFile("GameProviders/JDB/jdb.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseStartup<JDBStartup>()
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
    }
}
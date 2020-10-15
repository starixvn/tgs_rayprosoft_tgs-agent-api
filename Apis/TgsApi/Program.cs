using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using TGS.Apis.TgsApi.GameProviders.RedTiger;
using TGS.Apis.TgsApi.GameProviders.SBOBET;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TGS.Apis.TgsApi
{
    public class Program
    {
        private static readonly IReadOnlyDictionary<string, Type> StartupMap = new Dictionary<string, Type>
        {
            { Consts.GameProvider.RedTiger, typeof(RedTigerStartup) },
            { Consts.GameProvider.SBOBET, typeof(SBOBETStartup) },
        };

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await ExecuteMigraionsAndSeedingsAsync(host.Services);
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var gpCode = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.GameProviderCode) ?? string.Empty;
            var gpName = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.GameProviderName) ?? string.Empty;

            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostContext, config) =>
                    {
                        var path = $"{Directory.GetCurrentDirectory()}/GameProviders/{gpName}/{gpName.ToLower()}.json";
                        config.AddJsonFile(path, true);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        if (!StartupMap.TryGetValue(gpCode, out var startup))
                        {
                            startup = StartupMap.Values.ElementAt(0);
                        }

                        webBuilder.UseStartup(startup)
                                  .UseKestrel(options => options.AddServerHeader = false);
                    })
                    .ConfigureLogging((hostContext, logging) =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Trace);
                    })
                    .UseNLog();
        }

        private static async Task ExecuteMigraionsAndSeedingsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var isDevelopmentOrDocker = env.IsDevelopmentOrDocker();

            await using (var tgsRepo = sp.GetRequiredService<ITgsRepository>())
            {
                await tgsRepo.ExecuteMigrationAndSeedingAsync(isDevelopmentOrDocker);
            }

            await using (var betRepo = sp.GetRequiredService<IBetRepository>())
            {
                await betRepo.ExecuteMigrationAndSeedingAsync(isDevelopmentOrDocker);
            }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TGS.Apis.TgsApi
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await ExecuteMigrationsAndSeedingsAsync(host.Services);
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var gpCode = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.GameProviderCode) ?? string.Empty;
            var env = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.Environment) ?? string.Empty;

            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration(config =>
                    {
                        if (!ConfigFileMap.TryGetValue(gpCode, out var filePath))
                        {
                            filePath = ConfigFileMap.Values.ElementAt(0);
                        }

                        config.AddJsonFile($"{Directory.GetCurrentDirectory()}/GameProviders/{filePath}", true);
                        config.AddJsonFile($"{Directory.GetCurrentDirectory()}/GameProviders/{filePath}", true);
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
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Trace);
                    })
                    .UseNLog();
        }

        private static async Task ExecuteMigrationsAndSeedingsAsync(IServiceProvider services)
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
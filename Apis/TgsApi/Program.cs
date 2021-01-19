using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices.Bet;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Prometheus;
using TGS.Core.Models;

namespace TGS.Apis.TgsApi
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            PrometheusService.StartCollecting();

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
                    });
        }

        private static async Task ExecuteMigrationsAndSeedingsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var env = sp.GetRequiredService<IWebHostEnvironment>();

            await using (var tgsRepo = sp.GetRequiredService<ITgsRepository>())
            {
                await tgsRepo.ExecuteMigrationAndSeedingAsync(env);
            }

            await using (var betRepo = sp.GetRequiredService<IBetRepository>())
            {
                await betRepo.ExecuteMigrationAndSeedingAsync(env);
            }
        }
    }
}
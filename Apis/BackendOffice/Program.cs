using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.BackendOffice
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            await ExecuteMigrationsAndSeedingsAsync(host.Services);

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(builder=> 
                {
                    builder.AddDebug();
                    builder.AddConsole();
                });

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

            await using (var backOfficeRepo = sp.GetRequiredService<IBackendOfficeRepository>())
            { 
                await backOfficeRepo.ExecuteMigrationAndSeedingAsync(isDevelopmentOrDocker);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using TGS.Core.DataServices.Log;

namespace TGS.Apis.LogApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            await ExecuteMigrationAndSeedingAsync(host.Services);

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Error);
                    }
                });

        private static async Task ExecuteMigrationAndSeedingAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            await using var repo = scope.ServiceProvider.GetRequiredService<ILogRepository>();
            await repo.ExecuteMigrationAndSeedingAsync();
        }
    }
}

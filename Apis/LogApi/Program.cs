using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices.Log;
using TGS.Core.Infrastructures.Prometheus;

namespace TGS.Apis.LogApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            PrometheusService.StartCollecting();

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

        private static async Task ExecuteMigrationAndSeedingAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var env = sp.GetRequiredService<IWebHostEnvironment>();

            await using var tgsRepo = sp.GetRequiredService<ILogRepository>();
            await tgsRepo.ExecuteMigrationAndSeedingAsync(env);
        }
    }
}

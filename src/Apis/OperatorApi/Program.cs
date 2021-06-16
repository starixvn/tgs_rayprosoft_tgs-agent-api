using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Prometheus;
using Microsoft.Extensions.Logging;

namespace TGS.Apis.OperatorApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            PrometheusService.StartCollecting();

            var host = CreateHostBuilder(args).Build();
            await ExecuteMigrationsAndSeedingsAsync(host.Services);
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task ExecuteMigrationsAndSeedingsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var env = sp.GetRequiredService<IWebHostEnvironment>();

            await using var tgsRepo = sp.GetRequiredService<ITgsRepository>();
            await tgsRepo.ExecuteMigrationAndSeedingAsync(env);
        }
    }
}

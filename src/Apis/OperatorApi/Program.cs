using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices.Tgs;

namespace TGS.Apis.OperatorApi
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder.AddJsonFile("/config/appsettings.json", true).AddEnvironmentVariables();
                })
                .ConfigureLogging((ctx, logging) =>
                {
                    logging
                        .AddOpenTelemetry(options =>
                        {
                            options.IncludeFormattedMessage = true;
                            options.ParseStateValues = true;
                            _ = options.AddFluentdForwardExporter(fluentOptions =>
                            {
                                var nodeIp = ctx.Configuration.GetValue<string>("NODE_IP");
                                fluentOptions.Host = nodeIp;
                                fluentOptions.Tag = ctx.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
                                fluentOptions.UseMessagePack();
                            });
                        });
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

        public static async Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            var host = CreateHostBuilder(args).Build();
            await ExecuteMigrationsAndSeedingsAsync(host.Services);
            await host.RunAsync();
        }

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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Core.Infrastructures.Prometheus;

namespace TGS.Apis.BackendOffice
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
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
                            fluentOptions.BatchExportProcessorOptions.MaxExportBatchSize = 64 * 1024;
                        });
                    });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        public static async Task Main(string[] args)
        {
            PrometheusService.StartCollecting();

            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }
    }
}

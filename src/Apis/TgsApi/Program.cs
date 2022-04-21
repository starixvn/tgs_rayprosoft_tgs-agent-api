using System;
using System.IO;
using System.Threading.Tasks;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Core.Infrastructures.Prometheus;
using TGS.Core.Models;

namespace TGS.Apis.TgsApi
{
    public partial class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var gpCode = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.GameProviderCode) ??
                string.Empty;

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    if (!_ConfigFileMap.TryGetValue(gpCode, out var filePath))
                    {
                        throw new ArgumentException($"No configuration file mapping is found of '{gpCode}'");
                    }

                    config.AddJsonFile($"{Directory.GetCurrentDirectory()}/GameProviders/{filePath}", false)
                        .AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (!_StartupMap.TryGetValue(gpCode, out var startup))
                    {
                        throw new InvalidOperationException($"No startup configuration is found of '{gpCode}'");
                    }

                    webBuilder.UseStartup(startup)
                        .UseKestrel(options => options.AddServerHeader = false);
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
                .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
        }

        public static async Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            PrometheusService.StartCollecting();

            var builder = CreateHostBuilder(args).Build();

            await builder.RunAsync();
        }
    }
}

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TGS.Apis.AgentApi
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder.AddJsonFile("/config/appsettings.json", true).AddEnvironmentVariables();
                })
                .ConfigureLogging((ctx, builder) =>
                {
                    builder
                        .AddTgsLog()
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            CreateHostBuilder(args).Build().Run();
        }
    }
}
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGS.Apis.BackendOffice.Workers;

namespace TGS.Apis.BackendOffice;

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
                    });
                });
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<StationLetterBackground>();
            services.AddSingleton<IStationLetterTaskQueue>(_ =>
            {
                if (!int.TryParse(context.Configuration["QueueCapacity"], out var queueCapacity))
                {
                    queueCapacity = 100;
                }

                return new StationLetterTaskQueue(queueCapacity);
            });
        });

    public static async Task Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        SqlMapper.AddTypeMap(typeof(DateOnly), DbType.Date);

        var host = CreateHostBuilder(args).Build();

        await host.RunAsync();
    }
}

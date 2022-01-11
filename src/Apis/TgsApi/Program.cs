using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGS.Core.Infrastructures.Prometheus;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using Tgs.MessageQueue;

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

                    config.AddJsonFile($"{Directory.GetCurrentDirectory()}/GameProviders/{filePath}", false);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (!_StartupMap.TryGetValue(gpCode, out var startup))
                    {
                        throw new InvalidOperationException($"No startup configuration is found of '{gpCode}'");
                    }

                    webBuilder.UseStartup(startup)
                        .UseKestrel(options => options.AddServerHeader = false);
                });
        }

        public static Task Main(string[] args)
        {
            PrometheusService.StartCollecting();

            var builder = CreateHostBuilder(args).Build();

            _ = builder.Services.GetRequiredService<IRedisConnectionFactory>();

            return builder.RunAsync();
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public static Task Main(string[] args)
        {
            PrometheusService.StartCollecting();

            return CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var gpCode = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.GameProviderCode) ??
                         string.Empty;
            var env = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.Environment) ?? string.Empty;

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    if (!ConfigFileMap.TryGetValue(gpCode, out var filePath))
                    {
                        filePath = ConfigFileMap.Values.ElementAt(0);
                    }

                    config.AddJsonFile($"{Directory.GetCurrentDirectory()}/GameProviders/{filePath}", true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (!StartupMap.TryGetValue(gpCode, out var startup))
                    {
                        startup = StartupMap.Values.ElementAt(0);
                    }

                    webBuilder.UseStartup(startup)
                        .UseKestrel(options => options.AddServerHeader = false);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
        }
    }
}
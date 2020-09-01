using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TGS.Apis.TgsApi
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            return CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                           .ConfigureHostConfiguration(config =>
                           {
                               config.AddEnvironmentVariables();
                           })
                           .ConfigureWebHostDefaults(webBuilder =>
                           {
                               webBuilder.UseStartup<Startup>();
                           })
                           .ConfigureLogging((hostContext, logging) =>
                           {
                               logging.ClearProviders();
                               logging.SetMinimumLevel(LogLevel.Trace);
                           })
                           .UseNLog()
                           .ConfigureAppConfiguration((hostContext, config) =>
                           {
                               config.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                               config.AddJsonFile("appsettings.json", false, true)
                                   .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true)
                                   .AddEnvironmentVariables();
                               if (args != null)
                               {
                                   config.AddCommandLine(args);
                               }
                           });
        }
    }
}

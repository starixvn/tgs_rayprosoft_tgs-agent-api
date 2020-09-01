using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace TGS.Apis.TgsApi
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            //var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            //try
            //{
            //    logger.Debug("init main");
            //    //BuildWebHost(args).Build().Run();
            //}
            //catch (Exception ex)
            //{
            //    //NLog: catch setup errors
            //    logger.Error(ex, "Stopped program because of exception");
            //    throw;
            //}
            //finally
            //{
            //    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            //    NLog.LogManager.Shutdown();
            //}

            //NLog

            return CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                           .ConfigureHostConfiguration((config) =>
                           {
                               config.AddEnvironmentVariables();
                           })
                           .ConfigureLogging((hostContext, logging) =>
                           {
                               logging.ClearProviders();
                               logging.SetMinimumLevel(LogLevel.Trace);
                           })
                           .UseNLog()
                           .ConfigureWebHostDefaults(webBuilder =>
                           {
                               webBuilder.UseStartup<Startup>();
                           })
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
                               var logFactory = NLog.LogManager.LoadConfiguration($"nlog.{hostContext.HostingEnvironment.EnvironmentName}.config");
                               NLog.LogManager.Configuration = logFactory.Configuration;
                           });
        }
    }
}

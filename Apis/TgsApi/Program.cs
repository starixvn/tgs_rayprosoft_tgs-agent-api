using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

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
                           .ConfigureHostConfiguration((config) =>
                           {
                               config.AddEnvironmentVariables();
                           })
                           .ConfigureWebHostDefaults(webBuilder =>
                           {
                               webBuilder.UseStartup<Startup>();
                           })
                           .ConfigureAppConfiguration((hostContext, config) =>
                           {
                               config.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                               config.AddJsonFile("appsettings.json", false, true);
                               if (args != null)
                               {
                                   config.AddCommandLine(args);
                               }
                           })
                           .ConfigureLogging((hostcontext, loggin) =>
                           {
                               loggin.AddConfiguration(hostcontext.Configuration.GetSection("Logging"));
                               if (hostcontext.HostingEnvironment.IsDevelopment())
                               {
                                   loggin.AddConsole();
                                   loggin.SetMinimumLevel(LogLevel.Error);
                               }

                               loggin.AddConfiguration();
                           });
        }


    }
}

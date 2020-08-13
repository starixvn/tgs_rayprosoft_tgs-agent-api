using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TGS.Core.DataServices
{
    class Program
    {
        private static IHost _host;
        private static ILogger _logger;
        private static string _connectionString;

        private static async Task Main(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
            _logger = _host.Services.GetRequiredService<ILogger<Program>>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
                .AddJsonFile("appsettings.json");

            return null;
            //Host.CreateDefaultBuilder(args)
            //    .ConfigureHostConfiguration((config) => { config.AddEnvironmentVariables(); })
            //    .ConfigureAppConfiguration((hostContext, config) =>
            //    {
            //        config.SetBasePath(System.IO.Directory.GetCurrentDirectory());
            //        config.AddJsonFile("appsettings.json", false, true);
            //        if (args != null)
            //        {
            //            config.AddCommandLine(args);
            //        }
            //    })
            //    .ConfigureServices((hostcontext, services) =>
            //    {
            //        services.AddEFSecondLevelCache(options =>
            //            options.UseMemoryCacheProvider());

            //        _connectionString = hostcontext.Configuration.GetConnectionString("TGS");
            //        services.AddRicoDbContext(_connectionString);
            //    })
            //    .ConfigureLogging((hostcontext, loggin) =>
            //    {
            //        loggin.AddConfiguration(hostcontext.Configuration.GetSection("Logging"));
            //        if (hostcontext.HostingEnvironment.IsDevelopment())
            //        {
            //            loggin.AddConsole();
            //            loggin.SetMinimumLevel(LogLevel.Error);
            //        }
            //    });

        }
    }
}

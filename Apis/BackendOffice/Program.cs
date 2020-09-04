using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TGS.Apis.BackendOffice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var env = Environment.GetEnvironmentVariable(Core.Models.Consts.Environment);
                    config.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env}.json", true)
                        .AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                });
    }
}

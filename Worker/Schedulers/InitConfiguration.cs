using System;
using Microsoft.Extensions.Configuration;

namespace TGS.Worker.Schedulers
{
    public static class InitConfiguration
    {
        public static IConfigurationRoot Configuration;

        public static IConfigurationRoot Init()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
            {
                throw new ArgumentNullException("Environment not found in ASPNETCORE_ENVIRONMENT");
            }

            Console.WriteLine("Environment: {0}", environment);

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true);

            Configuration = builder.Build();
            return Configuration;
        }
    }
}
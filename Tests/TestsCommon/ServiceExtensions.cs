using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Log;

namespace TGS.Tests.TestsCommon
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddScoped<RequestAnalyzer, OsLog>();
            services.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();
            services.AddSingleton<IConfiguration>(configuration);
            return services;
        }
    }
}
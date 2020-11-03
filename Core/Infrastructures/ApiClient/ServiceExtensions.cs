using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.ApiClient
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of Infrastruces.ApiClient, need to add following dependencies beforehand:
        /// 1. AddJson() in Infrastructures.Json ( if BaseApiClient is used )
        /// 2. AddTgsLog() in Infrastructures.Log
        /// </summary>
        public static IServiceCollection AddApiClient(this IServiceCollection services)
        {
            services.AddScoped<IApiClientLogger, ApiClientLogger>();
            return services;
        }
    }
}
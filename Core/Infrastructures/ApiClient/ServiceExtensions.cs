using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.ApiClient
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of Infrastruce.ApiClient, need to add following dependencies beforehead:
        /// 1. AddJson() in Infrastructures.Json ( if BaseApiClient is used )
        /// 2. AddTgsLog() in Infrastructures.Log
        /// </summary>
        public static IServiceCollection AddApiClient(this IServiceCollection services)
        {
            services.AddSingleton<IApiClientLogger, ApiClientLogger>();
            return services;
        }
    }
}
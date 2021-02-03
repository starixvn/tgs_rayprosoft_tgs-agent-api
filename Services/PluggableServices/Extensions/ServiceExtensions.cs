using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.PluggableServices.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        ///     Dependency Injection of Operations, need to add the following dependencies beforehand:
        ///     1. AddOperations() in Infrastructures.Operations
        ///     2. AddApiClient() in Infrastructure.ApiClient
        /// </summary>
        public static IServiceCollection AddPluggableServices(this IServiceCollection services)
        {
            services.AddScoped<IPluggableOperations, PluggableOperations>();
            services.AddHttpClient<IPluggalbeGameProviderApiClient, PluggalbeGameProviderApiClient>();

            return services;
        }
    }
}

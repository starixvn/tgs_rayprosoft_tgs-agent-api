using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.Clients.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of Pluggable Game Provider Api Client, need to add the following dependencies beforehand:
        /// 1. AddApiClient() in Infrastructure.ApiClient
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPluggalbeGameProviderApiClient(this IServiceCollection services)
        {
            services.AddHttpClient<IPluggalbeGameProviderApiClient, PluggalbeGameProviderApiClient>();
            return services;
        }

    }
}

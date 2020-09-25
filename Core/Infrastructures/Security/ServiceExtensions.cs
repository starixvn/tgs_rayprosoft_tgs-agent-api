using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.Security
{
    public static class ServiceExtensions 
    {
        /// <summary>
        /// Dependency Injection of Infrastructure.Security
        /// </summary>
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddSingleton<ITgsHashConfig, TgsHashConfig>();
            services.AddSingleton<IHashProvider, HashProvider>();

            return services;
        }
    }
}

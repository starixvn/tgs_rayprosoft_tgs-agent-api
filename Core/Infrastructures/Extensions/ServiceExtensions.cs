using Microsoft.Extensions.DependencyInjection;

namespace TGS.Infrastructures.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructures(this IServiceCollection services)
        {
            services.AddSingleton<ITimeZoneManager, TimeZoneManager>();
            
            return services;
        }
    }
}

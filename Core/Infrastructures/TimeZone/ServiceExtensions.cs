using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.TimeZone
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTimeZone(this IServiceCollection services)
        {
            services.AddSingleton<ITimeZoneManager, TimeZoneManager>();            
            return services;
        } 
    }
}

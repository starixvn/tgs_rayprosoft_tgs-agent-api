using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestAnalyzer, RequestAnalyzer>();
            services.AddScoped<ILocalIpFinder, LocalIpFinder>();

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.WebApiServices.Filters;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services)
        {
            services.AddScoped<HandleTgsErrorFilter>();
            return services;
        }
    }
}
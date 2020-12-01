using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Services.BackOfficeApplication.DataService.Services;

namespace TGS.Services.BackOfficeApplication.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackOfficeApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient<IBrandService, BrandService>();

            return services;
        }
    }
}
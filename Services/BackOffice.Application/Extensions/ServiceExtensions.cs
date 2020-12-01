using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Services.BackOffice.Application.DataService.Services;

namespace TGS.Services.BackOffice.Application.Extensions
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
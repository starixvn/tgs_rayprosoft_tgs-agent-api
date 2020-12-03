using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Services.BackOffice.Application.DataService.Services;
using TGS.Services.BackOffice.Application.Middlewares;

namespace TGS.Services.BackOffice.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackOfficeApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UsernamePipe<,>));
            services.AddTransient<IBrandService, BrandService>();

            return services;
        }
    }
}
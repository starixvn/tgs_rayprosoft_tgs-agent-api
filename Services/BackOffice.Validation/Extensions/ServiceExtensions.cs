using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.BackOffice.Validation.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackOfficeApplicationValidatorServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
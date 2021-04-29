using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bonus.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBonusServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
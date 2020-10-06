using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of Infrastruce.ErrorMappers, it adds and resloves TgsErrorMapper as default;
        /// if you need to add custom error mappers, please DO Implement 'addCustom' and 'get' parameters
        /// </summary>
        public static IServiceCollection AddErrorMappers(this IServiceCollection services, Action addCustom = null, Func<IServiceProvider, Func<string, IErrorMapper>> get = null)
        {
            addCustom?.Invoke();
            
            services.AddSingleton<TgsErrorMapper>();

            if (get == null)
            {
                get = provider => key => provider.GetRequiredService<TgsErrorMapper>();
            }
            
            services.AddSingleton(provider => get(provider));

            return services;
        }
    }
}
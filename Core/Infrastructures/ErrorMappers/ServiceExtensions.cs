using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddErrorMappers(this IServiceCollection services, Action add, Func<IServiceProvider, Func<string, IErrorMapper>> get)
        {
            add();  
            
            services.AddSingleton(provider => get(provider));

            return services;
        }
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Log.Interfaces;

namespace TGS.Infrastructures.Log
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLogServices(this IServiceCollection services)
        {
            services.AddTransient<LogBusEvent>();
            services.AddSingleton(provider => new Func<LogBusEvent>(provider.GetService<LogBusEvent>));
            services.AddTransient<ITgsLog, TgsLog>();
            return services;
        }

    }
}
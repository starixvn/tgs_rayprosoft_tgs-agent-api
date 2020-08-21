using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Log.Events;

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
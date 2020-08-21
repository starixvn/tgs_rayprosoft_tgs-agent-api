using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Log.Interfaces;

namespace TGS.Infrastructures.Log
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLogServices(this IServiceCollection services, Action<LogBusEvent> logPublisher)
        {
            //services.AddTransient<Action<LogBusEvent>>((p) => logPublisher);
            services.AddTransient<ITgsLog, TgsLog>();
            return services;
        }

    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Helper;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Infrastructures.Nats;

namespace TGS.Core.Infrastructures.Log
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injections of Infrastructure.Log, needs to add the following dependencies beforehead:
        /// 1. AddWebApiServices() in Infrastructure.WebApiServices
        /// </summary>
        public static IServiceCollection AddTgsLog(this IServiceCollection services)
        {
            services.AddSingleton<LogBusEvent>(sp => new ApiLogEvent());
            services.AddSingleton<LogBusEvent>(sp => new LogRecordEvent());
            services.AddSingleton<LogBusEvent>(sp => new AdminLogEvent());
            services.AddScoped(c => CreateLoggerPublisher(c.GetRequiredService<INatsManager>()));    
    
            services.AddTransient<ITgsLog, TgsLog>();    
            services.AddTransient(typeof(IServiceLog<>), typeof(ServiceLog<>));

            services.AddLogging();

            return services;
        }

        private static Action<LogBusEvent> CreateLoggerPublisher(INatsManager natsManager)
        {
            var natsPublisher = CreateNatsPublisher(natsManager);

            return log => AsyncHelper.RunAsync(async s => await natsPublisher(s), log);
        }

        private static Func<object, Task> CreateNatsPublisher(INatsManager natsManager)
        {
            return log => natsManager.PublishAsync(log.GetType().Name, log);
        }
    }
}
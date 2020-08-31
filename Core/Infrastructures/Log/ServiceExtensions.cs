using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Helper;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Nats;

namespace TGS.Infrastructures.Log
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTgsLog(this IServiceCollection services)
        {
            services.AddTransient<ApiLogEvent>();
            services.AddTransient<LogRecordEvent>();
            services.AddTransient<LogBusEvent>(sp => new ApiLogEvent());
            services.AddTransient<LogBusEvent>(sp => new LogRecordEvent());
            services.AddTransient(p => CreateLoggerPublisher(p.GetService<INatsManager>()));
            services.AddTransient<ITgsLog, TgsLog>();

            return services;
        }

        public static Action<LogBusEvent> CreateLoggerPublisher(INatsManager natsManager)
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
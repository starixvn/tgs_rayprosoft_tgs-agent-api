using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Helper;
using TGS.Infrastructures.Log;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Nats;

namespace TGS.Apis.LogApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTgsLog(this IServiceCollection services)
        {
            services.AddTransient<Action<LogBusEvent>>(p => CreateLoggerPublisher(p.GetService<INatsManager>()));
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

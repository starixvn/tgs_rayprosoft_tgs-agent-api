using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Helper;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Log.Services;
using TGS.Infrastructures.Nats;

namespace TGS.Infrastructures.Log
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTgsLog(this IServiceCollection services)
        {
            services.AddScoped<LogBusEvent>(sp => new ApiLogEvent());
            services.AddScoped<LogBusEvent>(sp => new LogRecordEvent());
            services.AddScoped<LogBusEvent>(sp => new AdminLogEvent());
            services.AddScoped(p => CreateLoggerPublisher(p.GetService<INatsManager>()));
            services.AddScoped<ITgsLog, TgsLog>();
            services.AddTransient<Func<ITgsLog>>(c => () => c.GetService<ITgsLog>());

            services.AddScoped<IRequestAnalyzer, RequestAnalyzer>();
            services.AddScoped<ILocalIpFinder, LocalIpFinder>();
            services.AddTransient(typeof(IServiceLog<>), typeof(ServiceLog<>));

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
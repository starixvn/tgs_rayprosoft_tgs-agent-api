using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Helper;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Infrastructures.Log.Services;
using TGS.Core.Infrastructures.Nats;

namespace TGS.Core.Infrastructures.Log
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTgsLog(this IServiceCollection services)
        {
            services.AddSingleton<LogBusEvent>(sp => new ApiLogEvent());
            services.AddSingleton<LogBusEvent>(sp => new LogRecordEvent());
            services.AddSingleton<LogBusEvent>(sp => new AdminLogEvent());
            services.AddScoped(p => CreateLoggerPublisher(p.GetRequiredService<INatsManager>()));    
            
            services.AddScoped<IRequestAnalyzer, RequestAnalyzer>();
            services.AddScoped<ILocalIpFinder, LocalIpFinder>();
            services.AddTransient<ITgsLog, TgsLog>();    

            services.AddTransient(typeof(IServiceLog<>), typeof(ServiceLog<>));
            services.AddTransient<Func<ITgsLog>>(c => () => c.GetRequiredService<ITgsLog>());
            
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
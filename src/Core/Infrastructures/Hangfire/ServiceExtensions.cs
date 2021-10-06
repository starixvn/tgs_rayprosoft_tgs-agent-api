using System;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Console;
using Hangfire.Dashboard.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TGS.Core.Infrastructures.Hangfire.Services;

namespace TGS.Core.Infrastructures.Hangfire
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHangfireScheduler(
            this IServiceCollection services,
            IConfiguration configuration, 
            string configSectionName = "Hangfire", 
            BaseJobProvider jobProvider = null,
            Type[] typesToUseManagementPage = null)
        {
            services.Configure<HangfireSettings>(configuration.GetSection(configSectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<HangfireSettings>>().Value);

            services.AddHangfire((sp, option) =>
            {
                var settings = sp.GetRequiredService<HangfireSettings>();
                option.UseRedisStorage(settings.ConnectionString);

                option.UseSimpleAssemblyNameTypeSerializer()
                    .UseColouredConsoleLogProvider()
                    .UseConsole()
                    .UseRecommendedSerializerSettings(s =>
                    {
                        s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        s.TypeNameHandling = TypeNameHandling.None;
                        s.NullValueHandling = NullValueHandling.Include;
                        s.DefaultValueHandling = DefaultValueHandling.Include;
                    });

                if (typesToUseManagementPage?.Length > 0)
                {
                    option.UseManagementPages(p => p.AddJobs(typesToUseManagementPage));
                }
            });

            if (jobProvider == null)
            {
                return services;
            }

            services.AddSingleton<IJobProvider>(jobProvider);

            var jobs = ((IJobProvider) jobProvider).GetJobDescriptions();
            foreach (var job in jobs) services.AddTransient(job.Worker);

            return services;
        }
    }
}
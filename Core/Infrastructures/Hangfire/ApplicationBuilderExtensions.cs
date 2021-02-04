using System;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Hangfire.Services;

namespace TGS.Core.Infrastructures.Hangfire
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHangfireScheduler(this IApplicationBuilder app, string[] queues)
        {
            var setting = app.ApplicationServices.GetRequiredService<HangfireSettings>();

            var options = new BackgroundJobServerOptions
            {
                Queues = queues,
                SchedulePollingInterval = TimeSpan.FromSeconds(setting.Schedule.SchedulePollingInterval)
            };

            app.UseHangfireServer(options);

            var jobProvider = app.ApplicationServices.GetRequiredService<IJobProvider>();
            var manager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

            foreach (var job in jobProvider.GetJobDescriptions())
            {
                var jobId = job.Id.ToString();
                var method = job.Worker.GetMethod("ExecuteWorkerAsync");

                manager.RemoveIfExists(jobId);
                manager.AddOrUpdate(job.Id.ToString(), new(job.Worker, method, job.Name, job.Context), job.CronExpression, TimeZoneInfo.Utc);
            }

            return app;
        }
    }
}

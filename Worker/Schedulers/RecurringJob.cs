using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using TGS.Worker.Schedulers.Jobs;

namespace TGS.Worker.Schedulers
{
    public static class RecurringJob
    {
        public static void Jobs(IServiceProvider serviceProvider)
        {
            var manager = serviceProvider.GetService<IRecurringJobManager>();

            manager.AddOrUpdate<BrandGameProviderScheduleActivator>("BrandGameProvider",
                x => x.ChangeBrandGameProviderStatusAsync(),
                "* * * * * *");
        }
    }
}
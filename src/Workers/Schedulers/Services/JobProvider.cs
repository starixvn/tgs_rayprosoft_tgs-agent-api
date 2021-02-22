using System;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Workers.Schedulers.Jobs;
using TGS.Workers.Schedulers.Jobs.GameProviders.SBOBET;
using TGS.Workers.Schedulers.Jobs.Reports;

namespace TGS.Workers.Schedulers.Services
{
    public sealed class JobProvider : BaseJobProvider
    {
        public JobProvider() :
            base(
                new JobDescription(new Guid("500BD4EC-7B4A-4661-A00B-7C7BDC565CB4"), Cron.Minutely(), null,
                    typeof(BrandGameProviderScheduleWorker)),
                new JobDescription(new Guid("500BD4EC-7B4A-4661-A00B-7C7BDC666CB4"), "0 */5 * * * *", null,
                    typeof(MarketReportWorker)),
                new JobDescription(new("{EFB74D1F-355C-4298-9E60-33C003B16ECE}"), "*/5 * * * *", null,
                    typeof(CancelFailedTransactionsJobWorker))
                //,
                //new JobDescription(new("9CD49119-7BEA-465D-932A-6FAF6B39FE83"), Cron.Hourly(10), null, typeof(MarketReportWorker))
            )
        {
        }
    }
}
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Workers.Schedulers.Jobs;
using TGS.Workers.Schedulers.Jobs.Agents;
using TGS.Workers.Schedulers.Jobs.GameProviders.SBOBET;
using TGS.Workers.Schedulers.Jobs.Players;
using TGS.Workers.Schedulers.Jobs.Reports;
using TGS.Workers.Schedulers.Jobs.Rounds;

namespace TGS.Workers.Schedulers.Services
{
    public sealed class JobProvider : BaseJobProvider
    {
        public JobProvider() :
            base(
                new JobDescription(new("500BD4EC-7B4A-4661-A00B-7C7BDC565CB4"), Cron.Minutely(), null,
                    typeof(BrandGameProviderScheduleWorker)),
                new JobDescription(new("321E2BEE-7751-4D82-8CA7-BB1F1B83C4E0"), Cron.Daily(12, 00), null,
                    typeof(AgentDividendWorker)),
                new JobDescription(new("123E2BEE-7751-4D82-8CA7-BB1F1B83C4E0"), Cron.Daily(12, 00), null,
                    typeof(PlayerRebateWorker)),
                new JobDescription(new("888E2BEE-7751-4D82-8CA7-BB1F1B83C4E0"), "0 1 * * * *", null,
                    typeof(RoundAggregateWorker)),
                new JobDescription(new("999E2BEE-7751-4D82-8CA7-BB1F1B83C4E0"), "0 20 * * * *", null,
                    typeof(PlayerAggregateReportWorker)),
                new JobDescription(new("EFB74D1F-355C-4298-9E60-33C003B16ECE"), "0 */20 * * * *", null,
                    typeof(CancelFailedTransactionsJobWorker)))
        {
        }
    }
}
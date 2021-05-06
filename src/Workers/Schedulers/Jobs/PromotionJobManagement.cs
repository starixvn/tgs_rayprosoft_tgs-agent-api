using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Dashboard.Management.Support;
using Hangfire.Server;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Jobs.Promotion;

namespace TGS.Workers.Schedulers.Jobs
{
    [ManagementPage("活動手動Schedule", Consts.Queue.Schedule)]
    public class PromotionJobManagement
    {
        private readonly IPromotionSettlementDailyWorkerService _promotionSettlementDailyWorker;

        public PromotionJobManagement(IPromotionSettlementDailyWorkerService promotionSettlementDailyWorker)
        {
            _promotionSettlementDailyWorker = promotionSettlementDailyWorker;
        }


        [Queue(Consts.Queue.Schedule)]
        [Job]
        [DisplayName("create promotion dispatch")]
        [Description("建立每日派發資料")]
        [AutomaticRetry(Attempts = 3)]
        [DisableConcurrentExecution(90)]
        public async Task<TgsResponse> CreatePromotionDispatchAsync(
            [DisplayData("指定日期",
                "2021/02/18",
                "Default offset +08:00")]
            string dateTime = "",
            PerformContext context = null)
        {
            context.WriteLine($"Start time:{DateTime.Now}");

            if (DateTimeOffset.TryParse($"{dateTime} +8:00", out _))
            {
                return await _promotionSettlementDailyWorker.ManualExecutionAsync(dateTime);
            }

            return new() {ErrorMessage = "DateTime invalided"};
        }
    }
}
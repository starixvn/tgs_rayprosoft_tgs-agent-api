using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Dashboard.Management.Support;
using Hangfire.Server;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Jobs.Reports;

namespace TGS.Workers.Schedulers.Jobs
{
    [ManagementPage("手動Schedule", Consts.Queue.Schedule)]
    public class JobManagement
    {
        private readonly IMarketReportService _worker;

        public JobManagement(IMarketReportService worker)
        {
            _worker = worker;
        }

        [Job]
        [DisplayName("Test")]
        public void Action(PerformContext context = null, IJobCancellationToken cancellationToken = null)
        {
            context.WriteLine($"測試用，Now:{DateTime.Now}");
            Thread.Sleep(30000);
        }

        [Queue(Consts.Queue.Schedule)]
        [Job]
        [DisplayName("create or update market report")]
        [Description("更新市場報表指定的時間")]
        [AutomaticRetry(Attempts = 3)]
        [DisableConcurrentExecution(90)]
        public async Task<TgsResponse> UpdateMarketReportAsync(
            [DisplayData("指定日期",
                "2021/02/18",
                "Default offset +08:00")]
            string dateTime = "",
            [DisplayData("指定小時",
                "",
                "0~23")]
            int hour = 0,
            PerformContext context = null)
        {
            context.WriteLine($"Start time:{DateTime.Now}");

            if (DateTimeOffset.TryParse($"{dateTime} +8:00", out _))
            {
                return await _worker.ExecutionAsync(dateTime, hour);
            }

            return new TgsResponse
            {
                ErrorMessage = "DateTime invalided"
            };
        }
    }
}
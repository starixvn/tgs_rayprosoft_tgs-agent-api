using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Dashboard.Management.Support;
using Hangfire.Server;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Jobs.Reports;
using TGS.Workers.Schedulers.Jobs.Rounds;

namespace TGS.Workers.Schedulers.Jobs
{
    [ManagementPage("手動Schedule", Consts.Queue.Schedule)]
    public class JobManagement
    {
        private readonly IPlayerAggregateReportWorkerService _playerAggregateWorker;
        private readonly IRoundAggregateWorkerService _roundAggregateWorker;

        public JobManagement(IRoundAggregateWorkerService roundAggregateWorker,
            IPlayerAggregateReportWorkerService playerAggregateWorker)
        {
            _roundAggregateWorker = roundAggregateWorker;
            _playerAggregateWorker = playerAggregateWorker;
        }

        [Queue(Consts.Queue.Schedule)]
        [Job]
        [DisplayName("create or update round aggregate")]
        [Description("更新局集合指定的時間")]
        [AutomaticRetry(Attempts = 3)]
        [DisableConcurrentExecution(90)]
        public async Task<TgsResponse> UpdateRoundAggregateAsync(
            [DisplayData("指定日期",
                "2021/02/18",
                "Default offset +08:00")]
            string dateTime = "",
            [DisplayData("指定小時",
                "",
                "0~23, 13 = 13~14")]
            int hour = 0,
            PerformContext context = null)
        {
            context.WriteLine($"Start time:{DateTime.Now}");

            if (DateTimeOffset.TryParse($"{dateTime} +8:00", out _))
            {
                return await _roundAggregateWorker.ExecutionAsync(dateTime, hour);
            }

            return new() {ErrorMessage = "DateTime invalided"};
        }

        [Queue(Consts.Queue.Schedule)]
        [Job]
        [DisplayName("create or update player aggregate")]
        [Description("更新局集合指定的時間")]
        [AutomaticRetry(Attempts = 3)]
        [DisableConcurrentExecution(90)]
        public async Task<TgsResponse> UpdatePlayerAggregateAsync(
            [DisplayData("指定日期",
                "2021/02/18",
                "Default offset +08:00")]
            string dateTime = "",
            [DisplayData("指定小時",
                "",
                "0~23, 13 = 13~14")]
            int hour = 0,
            PerformContext context = null)
        {
            context.WriteLine($"Start time:{DateTime.Now}");

            if (DateTimeOffset.TryParse($"{dateTime} +8:00", out _))
            {
                return await _playerAggregateWorker.ManualExecutionAsync(dateTime, hour);
            }

            return new() {ErrorMessage = "DateTime invalided"};
        }
    }
}
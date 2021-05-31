using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Management.Metadata;
using Hangfire.Dashboard.Management.Support;
using Hangfire.Server;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Jobs.Agents;
using TGS.Workers.Schedulers.Jobs.Players;

namespace TGS.Workers.Schedulers.Jobs
{
    [ManagementPage("手動反水跟代理分紅", Consts.Queue.Schedule)]
    public class RebateAndDividendJobManagement
    {
        private readonly IAgentDividendWorkerService _agentDividendWorker;
        private readonly IPlayerRebateWorkerService _playerRebateWorker;

        public RebateAndDividendJobManagement(IPlayerRebateWorkerService playerRebateWorker,
            IAgentDividendWorkerService agentDividendWorker)
        {
            _playerRebateWorker = playerRebateWorker;
            _agentDividendWorker = agentDividendWorker;
        }

        [Queue(Consts.Queue.Schedule)]
        [Job]
        [DisplayName("create player rebates")]
        [Description("返水指定的時間")]
        [AutomaticRetry(Attempts = 3)]
        [DisableConcurrentExecution(90)]
        public async Task<TgsResponse> CreatePlayerRebateAsync(
            [DisplayData("指定日期",
                "2021/02/18",
                "Default offset +08:00")]
            string dateTime = "",
            PerformContext context = null)
        {
            context.WriteLine($"Start time:{DateTime.Now}");

            if (DateTimeOffset.TryParse($"{dateTime} +8:00", out _))
            {
                return await _playerRebateWorker.ManualExecutionAsync(dateTime);
            }

            return new() {ErrorMessage = "DateTime invalided"};
        }

        [Queue(Consts.Queue.Schedule)]
        [Job]
        [DisplayName("create agent dividends")]
        [Description("分紅指定的品牌")]
        [AutomaticRetry(Attempts = 3)]
        [DisableConcurrentExecution(90)]
        public async Task<TgsResponse> CreateAgentDividendsAsync(
            [DisplayData("指定品牌",
                "brand code",
                "TGS")]
            string brandCode = "",
            PerformContext context = null)
        {
            context.WriteLine($"Start time:{DateTime.Now}");

            return await _agentDividendWorker.ManualExecutionAsync(brandCode);
        }
    }
}
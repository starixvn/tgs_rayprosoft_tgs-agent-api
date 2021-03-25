using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    [Queue(Consts.Queue.Report)]
    public sealed class PlayerAggregateReportWorker : BaseWork<TgsResponse>
    {
        private readonly IPlayerAggregateReportWorkerService _worker;

        public PlayerAggregateReportWorker(
            IPlayerAggregateReportWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
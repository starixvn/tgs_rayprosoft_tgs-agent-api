using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    [Queue(Consts.Queue.Report)]
    public sealed class MarketAggregateReportWorker : BaseWork<TgsResponse>
    {
        private readonly IMarketAggregateReportWorkerService _worker;

        public MarketAggregateReportWorker(
            IMarketAggregateReportWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Rounds
{
    [Queue(Consts.Queue.Report)]
    public sealed class RoundAggregateWorker : BaseWork<TgsResponse>
    {
        private readonly IRoundAggregateWorkerService _worker;

        public RoundAggregateWorker(
            IRoundAggregateWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
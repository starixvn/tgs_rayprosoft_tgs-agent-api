using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Rounds
{
    [Queue(Consts.Queue.Report)]
    public sealed class FillPlayerBucketMinutelyWorker : BaseWork<TgsResponse>
    {
        private readonly IFillPlayerBucketMinutelyWorkerService _worker;

        public FillPlayerBucketMinutelyWorker(
            IFillPlayerBucketMinutelyWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
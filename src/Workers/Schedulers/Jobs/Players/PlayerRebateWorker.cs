using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Players
{
    [Queue(Consts.Queue.Report)]
    public sealed class PlayerRebateWorker : BaseWork<TgsResponse>
    {
        private readonly IPlayerRebateWorkerService _worker;

        public PlayerRebateWorker(
            IPlayerRebateWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
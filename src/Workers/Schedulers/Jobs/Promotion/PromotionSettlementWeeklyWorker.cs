using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Promotion
{
    [Queue(Consts.Queue.Report)]
    public sealed class PromotionSettlementWeeklyWorker : BaseWork<TgsResponse>
    {
        private readonly IPromotionSettlementWeeklyWorkerService _worker;

        public PromotionSettlementWeeklyWorker(
            IPromotionSettlementWeeklyWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
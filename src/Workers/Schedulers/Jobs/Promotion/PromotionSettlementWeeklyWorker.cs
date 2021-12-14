using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Hangfire.Services;

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
using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Hangfire.Services;

namespace TGS.Workers.Schedulers.Jobs.Promotion
{
    [Queue(Consts.Queue.Report)]
    public sealed class PromotionSettlementMonthlyWorker : BaseWork<TgsResponse>
    {
        private readonly IPromotionSettlementMonthlyWorkerService _worker;

        public PromotionSettlementMonthlyWorker(
            IPromotionSettlementMonthlyWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
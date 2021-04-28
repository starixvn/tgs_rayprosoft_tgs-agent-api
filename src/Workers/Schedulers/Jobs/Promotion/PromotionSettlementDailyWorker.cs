using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Promotion
{
    [Queue(Consts.Queue.Report)]
    public sealed class PromotionSettlementDailyWorker : BaseWork<TgsResponse>
    {
        private readonly IPromotionSettlementDailyWorkerService _worker;

        public PromotionSettlementDailyWorker(
            IPromotionSettlementDailyWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}
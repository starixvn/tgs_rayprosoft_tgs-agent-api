using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.Promotion.Clients;

namespace TGS.Workers.Schedulers.Jobs.Promotion
{
    public interface IPromotionSettlementDailyWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date);
    }

    public class PromotionSettlementDailyWorkerService : IPromotionSettlementDailyWorkerService
    {
        private readonly IPromotionBonusClient _client;

        public PromotionSettlementDailyWorkerService(
            IPromotionBonusClient client)
        {
            _client = client;
        }

        async Task<TgsResponse> IPromotionSettlementDailyWorkerService.ExecutionAsync()
        {
            var response = await _client.CreatePromotionSettleDataDailyAsync(new()).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }

        async Task<TgsResponse> IPromotionSettlementDailyWorkerService.ManualExecutionAsync(string date)
        {
            var response = await _client.CreatePromotionSettleDataDailyAsync(new(date)).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }
    }
}
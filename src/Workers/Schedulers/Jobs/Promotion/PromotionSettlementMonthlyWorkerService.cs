using System.Threading.Tasks;
using TGS.Clients.Promotion.Client;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Promotion
{
    public interface IPromotionSettlementMonthlyWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string from, string to);
    }

    public class PromotionSettlementMonthlyWorkerService : IPromotionSettlementMonthlyWorkerService
    {
        private readonly IPromotionBonusClient _client;

        public PromotionSettlementMonthlyWorkerService(
            IPromotionBonusClient client)
        {
            _client = client;
        }

        async Task<TgsResponse> IPromotionSettlementMonthlyWorkerService.ExecutionAsync()
        {
            var response = await _client.CreatePromotionSettleDataMonthlyAsync(new()).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }

        async Task<TgsResponse> IPromotionSettlementMonthlyWorkerService.ManualExecutionAsync(string from,
            string to)
        {
            var response = await _client.CreatePromotionSettleDataMonthlyAsync(new(from, to)).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }
    }
}
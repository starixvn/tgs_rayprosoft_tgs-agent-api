using System.Threading.Tasks;
using Report.Client;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Promotion
{
    public interface IPromotionSettlementWeeklyWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string from, string to);
    }

    public class PromotionSettlementWeeklyWorkerService : IPromotionSettlementWeeklyWorkerService
    {
        private readonly IPromotionSettlementClient _client;

        public PromotionSettlementWeeklyWorkerService(
            IPromotionSettlementClient client)
        {
            _client = client;
        }

        async Task<TgsResponse> IPromotionSettlementWeeklyWorkerService.ExecutionAsync()
        {
            var response = await _client.CreatePromotionSettleDataWeeklyAsync(new());
            return new() {ErrorMessage = response.Message};
        }

        async Task<TgsResponse> IPromotionSettlementWeeklyWorkerService.ManualExecutionAsync(string from,
            string to)
        {
            var response = await _client.CreatePromotionSettleDataWeeklyAsync(new(from, to));
            return new() {ErrorMessage = response.Message};
        }
    }
}
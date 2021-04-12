using System.Threading.Tasks;
using Report.Client;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Players
{
    public interface IPlayerRebateWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date = null);
    }

    public class PlayerRebateWorkerService : IPlayerRebateWorkerService
    {
        private readonly IReportAggregateClient _client;

        public PlayerRebateWorkerService(
            IReportAggregateClient client)
        {
            _client = client;
        }

        async Task<TgsResponse> IPlayerRebateWorkerService.ExecutionAsync()
        {
            var response = await _client.CreatePlayerRebateAsync(new());
            return new() {ErrorMessage = response.Message};
        }

        async Task<TgsResponse> IPlayerRebateWorkerService.ManualExecutionAsync(string date)
        {
            var response = await _client.CreatePlayerRebateAsync(new(date));
            return new() {ErrorMessage = response.Message};
        }
    }
}
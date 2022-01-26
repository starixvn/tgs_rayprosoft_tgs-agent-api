using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.ProfitRebate.Client;

namespace TGS.Workers.Schedulers.Jobs.Players
{
    public interface IPlayerRebateWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date = null);
    }

    public class PlayerRebateWorkerService : IPlayerRebateWorkerService
    {
        private readonly IProfitRebateClient _client;

        public PlayerRebateWorkerService(
            IProfitRebateClient client)
            => _client = client;

        async Task<TgsResponse> IPlayerRebateWorkerService.ExecutionAsync()
        {
            var response = await _client.CreatePlayerRebateAsync(new());
            return new() { ErrorMessage = response.Message };
        }

        async Task<TgsResponse> IPlayerRebateWorkerService.ManualExecutionAsync(string date)
        {
            var response = await _client.CreatePlayerRebateAsync(new(date));
            return new() { ErrorMessage = response.Message };
        }
    }
}
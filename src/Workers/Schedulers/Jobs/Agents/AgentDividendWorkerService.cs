using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.ProfitRebate.Client;
using Tgs.ProfitRebate.Client.Models;

namespace TGS.Workers.Schedulers.Jobs.Agents
{
    public interface IAgentDividendWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date = null);
    }

    public class AgentDividendWorkerService : IAgentDividendWorkerService
    {
        private readonly IProfitRebateClient _client;

        public AgentDividendWorkerService(
            IProfitRebateClient client)
            => _client = client;

        async Task<TgsResponse> IAgentDividendWorkerService.ExecutionAsync()
        {
            var response = await _client.CreateAgentDividendAsync(new(null));
            return new() { ErrorMessage = response?.Message };
        }

        async Task<TgsResponse> IAgentDividendWorkerService.ManualExecutionAsync(string brandCode)
        {
            var response =
                await _client.CreateAgentDividendAsync(new CreateAgentDividendsRequest(brandCode));
            return new() { ErrorMessage = response?.Message };
        }
    }
}
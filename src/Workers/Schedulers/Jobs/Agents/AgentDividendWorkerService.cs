using System.Threading.Tasks;
using TGS.Clients.Report.Client;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Agents
{
    public interface IAgentDividendWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date = null);
    }

    public class AgentDividendWorkerService : IAgentDividendWorkerService
    {
        private readonly IReportAggregateClient _client;

        public AgentDividendWorkerService(
            IReportAggregateClient client)
        {
            _client = client;
        }

        async Task<TgsResponse> IAgentDividendWorkerService.ExecutionAsync()
        {
            var response = await _client.CreateAgentDividendAsync(new());
            return new() {ErrorMessage = response.Message};
        }

        async Task<TgsResponse> IAgentDividendWorkerService.ManualExecutionAsync(string date)
        {
            var response = await _client.CreateAgentDividendAsync(new(date));
            return new() {ErrorMessage = response.Message};
        }
    }
}
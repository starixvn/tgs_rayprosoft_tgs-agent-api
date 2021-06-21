using System.Threading.Tasks;
using TGS.Clients.Report.Client;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Rounds
{
    public interface IRoundAggregateWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date, int hour);
    }

    public class RoundAggregateWorkerService : IRoundAggregateWorkerService
    {
        private readonly IReportAggregateClient _client;

        public RoundAggregateWorkerService(
            IReportAggregateClient client)
        {
            _client = client;
        }

        async Task<TgsResponse> IRoundAggregateWorkerService.ExecutionAsync()
        {
            var response = await _client.CreateRoundAggregateAsync(new()).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }

        async Task<TgsResponse> IRoundAggregateWorkerService.ManualExecutionAsync(string date, int hour)
        {
            var response = await _client.ManualCreateRoundAggregateAsync(new(date, hour)).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }
    }
}
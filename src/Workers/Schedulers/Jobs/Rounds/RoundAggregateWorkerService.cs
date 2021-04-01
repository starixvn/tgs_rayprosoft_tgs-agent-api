using System.Threading.Tasks;
using Report.Client;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Rounds
{
    public interface IRoundAggregateWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date = null, int hour = 0);
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
            var response = await _client.CreateUpdateRoundAggregateAsync(new()).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }

        async Task<TgsResponse> IRoundAggregateWorkerService.ManualExecutionAsync(string date, int hour)
        {
            var response = await _client.ManualCreateUpdateRoundAggregateAsync(new(date, hour)).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }
    }
}
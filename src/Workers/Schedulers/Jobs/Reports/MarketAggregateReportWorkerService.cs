using System.Threading.Tasks;
using TGS.Clients.Report.Client;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    public interface IMarketAggregateReportWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
    }

    public class MarketAggregateReportWorkerService : IMarketAggregateReportWorkerService
    {
        private readonly IReportAggregateClient _client;

        public MarketAggregateReportWorkerService(
            IReportAggregateClient client)
        {
            _client = client;
        }

        async Task<TgsResponse> IMarketAggregateReportWorkerService.ExecutionAsync()
        {
            var response = await _client.CreateMarketAggregateAsync(new()).ConfigureAwait(false);
            return new() {ErrorMessage = response.Message};
        }
    }
}
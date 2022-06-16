using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.Report.Aggregate.Client;

namespace TGS.Workers.Schedulers.Jobs.Reports;

public interface IPlayerAggregateReportWorkerService
{
    Task<TgsResponse> ExecutionAsync();
    Task<TgsResponse> ManualExecutionAsync(string date, int hour);
}

public class PlayerAggregateReportWorkerService : IPlayerAggregateReportWorkerService
{
    private readonly IReportAggregateClient _client;

    public PlayerAggregateReportWorkerService(
        IReportAggregateClient client)
        => _client = client;

    async Task<TgsResponse> IPlayerAggregateReportWorkerService.ExecutionAsync()
    {
        var response = await _client.CreatePlayerAggregateAsync(new())
            .ConfigureAwait(false);
        return new() { ErrorMessage = response.Message };
    }

    async Task<TgsResponse> IPlayerAggregateReportWorkerService.ManualExecutionAsync(string date, int hour)
    {
        var response = await _client.ManualCreatePlayerAggregateAsync(new(date, hour)).ConfigureAwait(false);
        return new() { ErrorMessage = response.Message };
    }
}
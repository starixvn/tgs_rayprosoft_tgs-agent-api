using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.ProfitRebate.Client;

namespace TGS.Workers.Schedulers.Jobs.Players;

public interface IPlayerAllAgentRebateWorkerService
{
    Task<TgsResponse> ExecutionAsync();
    Task<TgsResponse> ManualExecutionAsync(string date = null);
}

public class PlayerAllAgentRebateWorkerService : IPlayerAllAgentRebateWorkerService
{
    private readonly IProfitRebateClient _client;

    public PlayerAllAgentRebateWorkerService(
        IProfitRebateClient client)
        => _client = client;

    async Task<TgsResponse> IPlayerAllAgentRebateWorkerService.ExecutionAsync()
    {
        var response = await _client.CreatePlayerAllAgentRebateAsync(new(null));
        return new() { ErrorMessage = response?.Message };
    }

    async Task<TgsResponse> IPlayerAllAgentRebateWorkerService.ManualExecutionAsync(string date)
    {
        var response = await _client.CreatePlayerAllAgentRebateAsync(new(date));
        return new() { ErrorMessage = response?.Message };
    }
}
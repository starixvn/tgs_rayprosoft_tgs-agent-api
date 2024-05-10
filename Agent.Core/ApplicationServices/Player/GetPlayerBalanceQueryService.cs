using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class GetPlayerBalanceQueryService(
    IPlayerServiceClient playerServiceClient,
    ICashServiceClient cashServiceClient)
    : IGetService<GetPlayerBalanceQuery, decimal>
{
    private readonly ICashServiceClient _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
    private readonly IPlayerServiceClient _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));

    public async ValueTask<decimal> GetAsync(GetPlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var playerProps = await _playerServiceClient
            .GetPlayerPropertiesAsync(query.PlayerCode, cancellationToken)
            .ConfigureAwait(false);

        _ = playerProps.TryGetValue("AgentId", out var agentId);

        if (agentId is not null && agentId != query.AgentId.ToString())
            return 0M;

        var playerWalletId = Guid.Parse(playerProps["DefaultWalletId"]!);

        var balance = await _cashServiceClient
        .GetBalanceAsync(playerWalletId, cancellationToken)
        .ConfigureAwait(false);

        return balance;
    }
}

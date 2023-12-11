using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class GetAgentBalanceQueryService : IGetService<GetAgentBalanceQuery, decimal>
{
    private readonly ICashServiceClient _cashServiceClient;

    public GetAgentBalanceQueryService(ICashServiceClient cashServiceClient)
    {
        _cashServiceClient = cashServiceClient;
    }

    public async ValueTask<decimal> GetAsync(GetAgentBalanceQuery query, CancellationToken cancellationToken = default)
        => await _cashServiceClient
            .GetBalanceAsync(query.WalletId, cancellationToken)
            .ConfigureAwait(false);
}

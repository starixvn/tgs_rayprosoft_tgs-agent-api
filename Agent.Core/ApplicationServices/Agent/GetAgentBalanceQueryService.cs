using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class GetAgentBalanceQueryService(ICashServiceClient cashServiceClient)
    : IGetService<GetAgentBalanceQuery, decimal>
{
    public async ValueTask<decimal> GetAsync(GetAgentBalanceQuery query, CancellationToken cancellationToken = default)
        => await cashServiceClient
            .GetBalanceAsync(query.WalletId, cancellationToken)
            .ConfigureAwait(false);
}

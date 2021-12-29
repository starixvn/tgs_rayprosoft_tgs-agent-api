using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.WalletService;

namespace TGS.Services.Agent.ApplicationServices.Agent
{
    internal class GetAgentBalanceQueryService : IGetService<GetAgentBalanceQuery, decimal>
    {
        private readonly IAgentWalletRepository _agentWalletRepository;

        public GetAgentBalanceQueryService(IAgentWalletRepository agentWalletRepository)
        {
            _agentWalletRepository = agentWalletRepository ?? throw new ArgumentNullException(nameof(agentWalletRepository));
        }

        public async ValueTask<decimal> GetAsync(GetAgentBalanceQuery query, CancellationToken cancellationToken = default)
            => await _agentWalletRepository
                .GetBalanceAsync(query.WalletId, query.AgentId)
                .ConfigureAwait(false);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Wallet
{
    internal class GetBalanceQueryService : IGetService<GetBalanceQuery, BalanceInfo>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentWalletRepository _agentWalletRepository;

        public GetBalanceQueryService(
            IAgentRepository agentRepository,
            IAgentWalletRepository agentWalletRepository)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _agentWalletRepository = agentWalletRepository ?? throw new ArgumentNullException(nameof(agentWalletRepository));
        }

        public async ValueTask<BalanceInfo> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var agentData = await _agentRepository
                .GetAgentByIdAsync(query.AgentId)
                .ConfigureAwait(false);

            var balance = await _agentWalletRepository
                .GetBalanceAsync(agentData.DefaultWalletId, query.AgentId)
                .ConfigureAwait(false);

            return new BalanceInfo(balance);
        }
    }
}

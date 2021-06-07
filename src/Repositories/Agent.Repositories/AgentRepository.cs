using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Agent;

namespace TGS.Agent.Repositories
{
    internal class AgentRepository : IAgentRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public AgentRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<AgentInfo> GetAgentByIdAsync(Guid id)
        {
            var agent = await _tgsRepository.Agents
                .FindAsync(id)
                .ConfigureAwait(false);

            return new AgentInfo(
                agent.Id,
                agent.Username,
                agent.DefaultWalletId,
                agent.WithdrawPassword,
                agent.WithdrawSalt);
        }
    }
}

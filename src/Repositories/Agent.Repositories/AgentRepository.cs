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
                agent.Email,
                agent.WithdrawPassword,
                agent.WithdrawSalt);
        }

        public async ValueTask SetPayPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn)
        {
            var agent = await _tgsRepository.Agents
                .FindAsync(agentId)
                .ConfigureAwait(false);

            agent.WithdrawPassword = newPassword;
            agent.WithdrawSalt = newSalt;
            agent.UpdatedOn = updatedOn;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

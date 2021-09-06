using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

            var dividend = await _tgsRepository.AgentDividends.AsQueryable()
                .Where(x => x.AgentId == id && x.SentOn == null)
                .SumAsync(x => x.AgentDividendAmount).ConfigureAwait(false);

            return new AgentInfo(
                agent.Id,
                agent.Username,
                agent.RealName,
                agent.Phone,
                agent.DefaultWalletId,
                agent.Email,
                agent.QQ,
                agent.Wechat,
                agent.InvitationCode,
                agent.Brand.Domains.FirstOrDefault()?.Url,
                (int)agent.LevelType,
                agent.WithdrawPassword,
                agent.WithdrawSalt,
                dividend,
                agent.DividendPercentage);
        }

        public async ValueTask SetPayPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt,
            DateTimeOffset updatedOn)
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
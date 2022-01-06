using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.Agent;

namespace TGS.Agent.Repositories
{
    internal class AgentRepository : IAgentRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public AgentRepository(ITgsRepository tgsRepository)
            => _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

        public async ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id)
        {
            var agent = await _tgsRepository.Agents
                .FindAsync(id)
                .ConfigureAwait(false);

            return agent == null
                ? null
                : new AgentInfo(
                    agent.Id,
                    agent.Username,
                    agent.RealName,
                    agent.Phone,
                    agent.DefaultWalletId,
                    agent.Email,
                    agent.QQ,
                    agent.Wechat,
                    agent.InvitationCode,
                    agent.WithdrawPassword,
                    agent.WithdrawSalt,
                    agent.Password,
                    agent.Salt,
                    agent.ParentAgentId,
                    agent.RegisteredOn,
                    agent.Status,
                    agent.Type,
                    agent.LevelType,
                    agent.DividendPercentage);
        }

        public IAsyncEnumerable<string> GetAgentDomainsByIdAsync(Guid agentId)
            => _tgsRepository.AgentDomains.AsQueryable()
                .Where(ad => ad.AgentId == agentId)
                .Select(ad => ad.DomainName)
                .ToAsyncEnumerable();

        public IAsyncEnumerable<AgentInfo> GetAgentsByEmailAsync(Guid brandId, string email)
        {
            var query = from agent in _tgsRepository.Agents.AsQueryable()
                        where agent.BrandId == brandId
                            && agent.Email.ToLower() == email.ToLower()
                        select new AgentInfo(
                            agent.Id,
                            agent.Username,
                            agent.RealName,
                            agent.Phone,
                            agent.DefaultWalletId,
                            agent.Email,
                            agent.QQ,
                            agent.Wechat,
                            agent.InvitationCode,
                            agent.WithdrawPassword,
                            agent.WithdrawSalt,
                            agent.Password,
                            agent.Salt,
                            agent.ParentAgentId,
                            agent.RegisteredOn,
                            agent.Status,
                            agent.Type,
                            agent.LevelType,
                            agent.DividendPercentage);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<AgentInfo> GetAgentsByPhoneAsync(Guid brandId, string phone)
        {
            var query = from agent in _tgsRepository.Agents.AsQueryable()
                        where agent.BrandId == brandId
                            && agent.Phone.ToLower() == phone.ToLower()
                        select new AgentInfo(
                            agent.Id,
                            agent.Username,
                            agent.RealName,
                            agent.Phone,
                            agent.DefaultWalletId,
                            agent.Email,
                            agent.QQ,
                            agent.Wechat,
                            agent.InvitationCode,
                            agent.WithdrawPassword,
                            agent.WithdrawSalt,
                            agent.Password,
                            agent.Salt,
                            agent.ParentAgentId,
                            agent.RegisteredOn,
                            agent.Status,
                            agent.Type,
                            agent.LevelType,
                            agent.DividendPercentage);

            return query.ToAsyncEnumerable();
        }

        public async ValueTask<AgentStatus> GetAgentStatusByIdAsync(Guid id)
        {
            var agent = await _tgsRepository.Agents.FindAsync(id).ConfigureAwait(false);

            return agent.Status;
        }

        public IAsyncEnumerable<DividendCondition> QueryAgentDividendConditionsAsync(Guid agentId)
        {
            var query = from agent in _tgsRepository.Agents.AsQueryable()
                        from dc in agent.AgentDividendConditions
                        where agent.Id == agentId
                        select new DividendCondition(
                            dc.Rank,
                            dc.NetProfit,
                            dc.ActiveNumber,
                            dc.DividendPercentage);

            return query.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<KeyValuePair<string, string>> QueryAllLocalizationStringsAsync(string cultureCode)
        {
            var rows = _tgsRepository.SystemLocalizations.AsQueryable()
                .Where(loc => loc.SystemCode == "AGENT-API" && loc.CultureCode == cultureCode)
                .Select(loc => new
                {
                    loc.MessageId,
                    loc.Text
                });

            return rows.ToAsyncEnumerable()
                .Select(item => new KeyValuePair<string, string>(item.MessageId, item.Text));
        }

        public async ValueTask SetPasswordAsync(
            Guid agentId, byte[] newPassword, byte[] newSalt, DateTimeOffset updatedOn)
        {
            var agent = await _tgsRepository.Agents
                .FindAsync(agentId)
                .ConfigureAwait(false);

            agent.Password = newPassword;
            agent.Salt = newSalt;
            agent.UpdatedOn = updatedOn;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask SetPayPasswordAsync(
            Guid agentId,
            byte[] newPassword,
            byte[] newSalt,
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

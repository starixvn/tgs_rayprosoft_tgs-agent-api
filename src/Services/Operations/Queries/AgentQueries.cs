using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Operations.Queries
{
    public interface IAgentQueries
    {
        Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> predicate,
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery = null);

        Task<AgentLoginRecord> GetAgentLoginRecordAsync(Expression<Func<AgentLoginRecord, bool>> predicate,
            Func<IQueryable<AgentLoginRecord>, IQueryable<AgentLoginRecord>> preQuery = null);

        Task<Agent> GetAgentByInvitationCodeAsync(string invitationCode);
    }

    public class AgentQueries : IAgentQueries
    {
        private readonly ITgsRepository _repository;

        public AgentQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<Agent> IAgentQueries.GetAgentAsync(Expression<Func<Agent, bool>> predicate,
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery)
        {
            preQuery ??= c => c;

            return preQuery(_repository.Agents).FirstOrDefaultAsync(predicate);
        }

        Task<Agent> IAgentQueries.GetAgentByInvitationCodeAsync(string invitationCode) =>
            _repository.Agents.FirstOrDefaultAsync(x => x.InvitationCode == invitationCode);

        Task<AgentLoginRecord> IAgentQueries.GetAgentLoginRecordAsync(
            Expression<Func<AgentLoginRecord, bool>> predicate,
            Func<IQueryable<AgentLoginRecord>, IQueryable<AgentLoginRecord>> preQuery)
        {
            preQuery ??= c => c;

            return preQuery(_repository.AgentLoginRecords).FirstOrDefaultAsync(predicate);
        }
    }
}
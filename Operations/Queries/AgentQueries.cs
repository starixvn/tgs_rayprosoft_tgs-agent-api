using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Queries;

public class AgentQueries(ITgsRepository repository) : IAgentQueries
{
    Task<Agent> IAgentQueries.GetAgentAsync(Expression<Func<Agent, bool>> predicate,
        Func<IQueryable<Agent>, IQueryable<Agent>> preQuery)
    {
        preQuery ??= c => c;

        return preQuery(repository.Agents).FirstOrDefaultAsync(predicate);
    }

    Task<Agent> IAgentQueries.GetAgentByInvitationCodeAsync(Guid brandId, string invitationCode)
        => repository.Agents
            .Where(agent => agent.BrandId == brandId
                && agent.InvitationCode == invitationCode)
            .FirstOrDefaultAsync();

    Task<AgentLoginRecord> IAgentQueries.GetAgentLoginRecordAsync(
        Expression<Func<AgentLoginRecord, bool>> predicate,
        Func<IQueryable<AgentLoginRecord>, IQueryable<AgentLoginRecord>> preQuery)
    {
        preQuery ??= c => c;

        return preQuery(repository.AgentLoginRecords).FirstOrDefaultAsync(predicate);
    }
}

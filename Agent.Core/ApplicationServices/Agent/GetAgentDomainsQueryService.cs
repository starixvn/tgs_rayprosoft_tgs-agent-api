using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class GetAgentDomainsQueryService(IAgentRepository agentRepository)
    : IQueryService<GetAgentDomainsQuery, string>
{
    private readonly IAgentRepository _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

    public IAsyncEnumerable<string> QueryAsync(GetAgentDomainsQuery query, CancellationToken cancellationToken = default)
        => _agentRepository.GetAgentDomainsByIdAsync(query.AgentId);
}

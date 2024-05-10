using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class GetAgentDividendConditionsByIdQueryService(IAgentRepository agentRepository)
    : IQueryService<GetAgentDividendConditionsByIdQuery, DividendCondition>
{
    private readonly IAgentRepository _agentRepository =
        agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

    public IAsyncEnumerable<DividendCondition> QueryAsync(
        GetAgentDividendConditionsByIdQuery query,
        CancellationToken cancellationToken = default)
        => _agentRepository.QueryAgentDividendConditionsAsync(query.AgentId);
}
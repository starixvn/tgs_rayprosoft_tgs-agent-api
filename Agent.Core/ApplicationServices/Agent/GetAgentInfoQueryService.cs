using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class GetAgentInfoQueryService(IAgentRepository agentRepository) : IGetService<GetAgentInfoQuery, AgentInfo?>
{
    private readonly IAgentRepository _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

    public ValueTask<AgentInfo?> GetAsync(GetAgentInfoQuery query, CancellationToken cancellationToken = default)
    {
        return _agentRepository.GetAgentByIdAsync(query.AgentId);
    }
}

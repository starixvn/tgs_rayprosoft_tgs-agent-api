using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class GetAgentStatusQueryService(IAgentRepository agentRepository)
    : IGetService<GetAgentStatusQuery, AgentStatus>
{
    private readonly IAgentRepository _agentRepository =
        agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

    public ValueTask<AgentStatus> GetAsync(GetAgentStatusQuery query, CancellationToken cancellationToken = default)
        => _agentRepository.GetAgentStatusByIdAsync(query.AgentId);
}
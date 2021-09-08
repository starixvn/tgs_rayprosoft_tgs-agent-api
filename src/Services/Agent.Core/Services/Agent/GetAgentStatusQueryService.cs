using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class GetAgentStatusQueryService : IGetService<GetAgentStatusQuery, AgentStatus>
    {
        private readonly IAgentRepository _agentRepository;

        public GetAgentStatusQueryService(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        }

        public ValueTask<AgentStatus> GetAsync(GetAgentStatusQuery query, CancellationToken cancellationToken = default)
            => _agentRepository.GetAgentStatusByIdAsync(query.AgentId);
    }
}
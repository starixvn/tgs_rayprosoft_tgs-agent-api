using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Agent
{
    internal class GetAgentInfoQueryService : IGetService<GetAgentInfoQuery, AgentInfo>
    {
        private readonly IAgentRepository _agentRepository;

        public GetAgentInfoQueryService(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        }

        public ValueTask<AgentInfo> GetAsync(GetAgentInfoQuery query, CancellationToken cancellationToken = default)
        {
            return _agentRepository.GetAgentByIdAsync(query.AgentId);
        }
    }
}

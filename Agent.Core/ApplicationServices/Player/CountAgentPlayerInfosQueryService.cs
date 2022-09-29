using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class CountAgentPlayerInfosQueryService : IGetService<GetAgentPlayerInfosQuery, int>
{
    private readonly IAgentRepository _agentRepository;

    public CountAgentPlayerInfosQueryService(
        IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
    }

    public ValueTask<int> GetAsync(
        GetAgentPlayerInfosQuery query,
        CancellationToken cancellationToken = default)
        => _agentRepository
            .QueryAgentPlayerInfosTotalCountAsync(
                query.AgentId,
                query.Username,
                query.BeginRegisteredOn,
                query.EndRegisteredOn,
                query.RegisterHost,
                query.Status,
                query.VipId);
}

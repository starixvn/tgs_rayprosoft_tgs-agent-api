using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

internal class GetSubAgentDividendRangeQueryService(IAgentClient agentClient)
    : IGetService<GetSubAgentDividendRangeQuery, (decimal, decimal)>
{
    private readonly IAgentClient _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));

    public async ValueTask<(decimal, decimal)> GetAsync(GetSubAgentDividendRangeQuery query,
        CancellationToken cancellationToken = default)
    {
        var childInfos = await _agentClient
            .GetChildAgentInfosAsync(new GetChildAgentInfosRequest(query.SubAgentId), cancellationToken)
            .ToArrayAsync(cancellationToken);

        var min = childInfos.Any()
            ? childInfos.Max(x => x.DividendPercentage)
            : 0M;

        var parentAgent = await _agentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(query.AgentId), cancellationToken)
            .ConfigureAwait(false);

        return (min, parentAgent!.DividendPercentage);
    }
}
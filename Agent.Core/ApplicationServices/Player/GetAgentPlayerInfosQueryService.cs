using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Paging;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class GetAgentPlayerInfosQueryService(IAgentRepository agentRepository)
    : IQueryService<GetAgentPlayerInfosQuery, AgentPlayerInfo>,
        IGetService<GetAgentPlayerInfosQuery, int>
{
    private readonly IAgentRepository _agentRepository =
        agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

    public async ValueTask<int> GetAsync(GetAgentPlayerInfosQuery query, CancellationToken cancellationToken = default)
    {
        var data = MakeDataSourceAsync(query);

        return await data.CountAsync(cancellationToken).ConfigureAwait(false);
    }

    public IAsyncEnumerable<AgentPlayerInfo> QueryAsync(
        GetAgentPlayerInfosQuery query,
        CancellationToken cancellationToken = default)
    {
        var data = MakeDataSourceAsync(query);

        return data.ApplyOrderBy(query.Paging).ApplyPaging(query.Paging);
    }

    private IAsyncEnumerable<AgentPlayerInfo> MakeDataSourceAsync(GetAgentPlayerInfosQuery query)
        => _agentRepository.QueryAgentPlayerInfosAsync(query.AgentId,
            query.Username,
            query.BeginRegisteredOn,
            query.EndRegisteredOn,
            query.RegisterHost,
            query.Status,
            query.VipId);
}
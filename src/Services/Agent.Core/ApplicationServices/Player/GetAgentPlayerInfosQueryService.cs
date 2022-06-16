using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;
using TGS.Paging;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class GetAgentPlayerInfosQueryService : IQueryService<GetAgentPlayerInfosQuery, AgentPlayerInfo>
{
    private readonly IAgentRepository _agentRepository;

    public GetAgentPlayerInfosQueryService(
        IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
    }

    public IAsyncEnumerable<AgentPlayerInfo> QueryAsync(
        GetAgentPlayerInfosQuery query,
        CancellationToken cancellationToken = default)
    {
        var paging = new Paging<AgentPlayerInfo>
        {
            Page = query.Paging.Page,
            RowCount = query.Paging.RowCount
        };

        if (!string.IsNullOrEmpty(query.Paging.SortBy))
        {
            paging.SortByPropertyName(query.Paging.SortBy, query.Paging.IsDesc);
        }

        return _agentRepository.QueryAgentPlayerInfosAsync(
            query.AgentId,
            query.Username,
            query.BeginRegisteredOn,
            query.EndRegisteredOn,
            query.RegisterHost,
            query.Status,
            query.VipId,
            paging);
    }
}

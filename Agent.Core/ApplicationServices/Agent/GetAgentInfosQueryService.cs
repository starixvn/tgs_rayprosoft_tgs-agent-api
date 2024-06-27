using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Agent;

internal class GetAgentInfosQueryService(ITgsRepository tgsRepository) : IQueryService<GetAgentInfosQuery, AgentInfos>
{
    private readonly ITgsRepository _tgsRepository =
        tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public IAsyncEnumerable<AgentInfos> QueryAsync(
        GetAgentInfosQuery query,
        CancellationToken cancellationToken = default)
    {
        var agents = from agent in _tgsRepository.Agents.AsQueryable()
                     where agent.BrandId == query.BrandId
                           && agent.ParentAgentId == query.ParentAgentId
                     select new AgentInfos
                     {
                         AgentStatus = agent.AgentStatus,
                         WalletId = agent.WalletId.Value,
                         CardGameRebatePercentage = agent.CardGameRebatePercentage,
                         CommissionPercentage = agent.CommissionPercentage,
                         DividendPercentage = agent.DividendPercentage,
                         LevelType = agent.LevelType,
                         LiveDealerRebatePercentage = agent.LiveDealerRebatePercentage,
                         LotteryRebatePercentage = agent.LotteryRebatePercentage,
                         RegisteredOn = agent.RegisteredOn,
                         SlotRebatePercentage = agent.SlotRebatePercentage,
                         SportsRebatePercentage = agent.SportsRebatePercentage,
                         Status = agent.Status,
                         Type = agent.Type,
                         Username = agent.Username,
                         VirtualSportsRebatePercentage = agent.VirtualSportsRebatePercentage,
                         AgentId = agent.Id
                     };

        if (query.Status != null)
            agents = agents.Where(x => x.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Username))
            agents = agents.Where(x => x.Username == query.Username);

        return agents.ToAsyncEnumerable();
    }
}
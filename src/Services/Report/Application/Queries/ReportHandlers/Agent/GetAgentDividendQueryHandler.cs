using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Agent
{
    internal class
        GetAgentDividendQueryHandler : Handler<GetAgentDividendRequest, PagedResponse<AgentDividendModel>>
    {
        private readonly IPlayerAggregateService _service;
        private readonly ITgsAgentService _tgsAgentService;

        public GetAgentDividendQueryHandler(IPlayerAggregateService service, ITgsAgentService tgsAgentService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsAgentService = tgsAgentService;
        }

        public override async Task<PagedResponse<AgentDividendModel>> Handle(
            GetAgentDividendRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _tgsAgentService.GetAgentAsync(x =>
                x.BrandId == request.BrandId && x.Username == request.Username);

            if (agent == null)
            {
                return CreateInvalidResponse<PagedResponse<AgentDividendModel>>(Resources.Agent.NotFound);
            }

            var agents = await _tgsAgentService
                .GetAgentQueries(q => q.Where(x => x.ParentAgentId == agent.Id || x.Id == agent.Id))
                .Include(x => x.SubAgents)
                .ToArrayAsync(cancellationToken);
            var agentIds = agents.Select(x => x.Id).ToArray();

            var agentBonus = await _service.GetPlayerAggregateQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.AgentId == agent.Id || agentIds.Contains(x.AgentId.Value))
                .Where(x => x.ReportDate >= request.From && x.ReportDate <= request.To)
                .GroupBy(x => new {x.PlayerCode, x.BrandId, x.AgentId},
                    (x, pa) => new
                    {
                        x.AgentId,
                        x.BrandId,
                        x.PlayerCode,
                        DepositAmount = pa.Sum(p => p.DepositAmount),
                        WithdrawalAmount = pa.Sum(p => p.WithdrawalAmount),
                        AdjustmentAmount = pa.Sum(p => p.AdjustmentAmount),
                        BonusAmount = pa.Sum(p => p.BonusAmount),
                        BetAmount = pa.Sum(p => p.BetAmount),
                        Turnover = pa.Sum(p => p.Turnover),
                        WinLossAmount = pa.Sum(p => p.WinLossAmount),
                        NetProfit = pa.Sum(p => p.NetProfit)
                    }).GroupJoin(_service.Repository.PlayerGameTypeAggregates
                        .Where(x => x.BrandId == request.BrandId)
                        .Where(x => x.AgentId == agent.Id || agentIds.Contains(x.AgentId.Value))
                        .Where(x => x.ReportDate >= request.From && x.ReportDate < request.To),
                    p => new {p.BrandId, p.PlayerCode, p.AgentId},
                    pg => new {pg.BrandId, pg.PlayerCode, pg.AgentId},
                    (p, pg) => new {p, pg})
                .SelectMany(x => x.pg.DefaultIfEmpty(),
                    (x, pg) => new
                    {
                        x.p.AgentId,
                        x.p.BrandId,
                        x.p.PlayerCode,
                        x.p.DepositAmount,
                        x.p.WithdrawalAmount,
                        x.p.AdjustmentAmount,
                        x.p.BonusAmount,
                        x.p.BetAmount,
                        x.p.Turnover,
                        x.p.WinLossAmount,
                        x.p.NetProfit,
                        pg
                    }).ToArrayAsync(cancellationToken);

            var bonus = agentBonus
                .Join(agents, ab => ab.AgentId, a => a.Id, (ab, a) => new {agentBounus = ab, agent = a})
                .GroupBy(
                    x => new
                    {
                        x.agentBounus.AgentId,
                        x.agent.Username,
                        x.agentBounus.BrandId,
                        x.agentBounus.PlayerCode,
                        x.agentBounus.DepositAmount,
                        x.agentBounus.WithdrawalAmount,
                        x.agentBounus.AdjustmentAmount,
                        x.agentBounus.BonusAmount,
                        x.agentBounus.BetAmount,
                        x.agentBounus.Turnover,
                        x.agentBounus.WinLossAmount,
                        x.agentBounus.NetProfit,
                        x.agent
                    },
                    (x, agents) =>
                    {
                        return new
                        {
                            id = x.AgentId,
                            x.agent,
                            x.Username,
                            x.DepositAmount,
                            x.WithdrawalAmount,
                            x.AdjustmentAmount,
                            x.BonusAmount,
                            x.BetAmount,
                            x.Turnover,
                            x.WinLossAmount,
                            x.NetProfit,
                            AgentDividendAmount = x.agent.DividendPercentage * (x.WinLossAmount - x.AdjustmentAmount -
                                x.BonusAmount - agents
                                    .Sum(m => m.agentBounus.pg.VipRebateAmount)) / 100
                        };
                    })
                .GroupBy(x => new {x.id, x.Username, x.agent},
                    (x, models) => new AgentDividendModel
                    {
                        id = x.id,
                        Username = x.Username,
                        DepositAmount = models.Sum(m => m.DepositAmount),
                        WithdrawalAmount = models.Sum(m => m.WithdrawalAmount),
                        AdjustmentAmount = models.Sum(m => m.AdjustmentAmount),
                        BonusAmount = models.Sum(m => m.BonusAmount),
                        BetAmount = models.Sum(m => m.BetAmount),
                        Turnover = models.Sum(m => m.Turnover),
                        WinLossAmount = models.Sum(m => m.WinLossAmount),
                        NetProfit = models.Sum(m => m.NetProfit),
                        AgentDividendAmount = models.Sum(m => m.AgentDividendAmount) +
                            x.agent.SubAgents.Sum(s =>
                            {
                                var childAgentPlayers = agentBonus
                                    .Where(a =>
                                        a.AgentId == s.Id).GroupBy(
                                        g => new
                                        {
                                            g.AgentId,
                                            g.PlayerCode,
                                            g.WinLossAmount,
                                            g.BonusAmount,
                                            g.AdjustmentAmount
                                        },
                                        (agent, childAgents) => new
                                        {
                                            agent.AgentId,
                                            agent.WinLossAmount,
                                            agent.BonusAmount,
                                            agent.AdjustmentAmount,
                                            RebateAmount = childAgents.Sum(c => c.pg.VipRebateAmount)
                                        }).ToArray();
                                return childAgentPlayers.Sum(c =>
                                        c.WinLossAmount - c.BonusAmount - c.AdjustmentAmount - c.RebateAmount) *
                                    (x.agent.DividendPercentage - s.DividendPercentage) / 100;
                            })
                    }).ToArray();

            return await bonus.CreatePagedResponseAsync(bonus.Length);
        }
    }
}
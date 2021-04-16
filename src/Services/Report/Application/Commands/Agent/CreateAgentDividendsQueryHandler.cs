using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.Agent
{
    public class
        CreateAgentDividendsQueryHandler : Handler<CreateAgentDividendsRequest,
            MessageResponse>
    {
        private readonly IAgentDividendService _agentDividendService;
        private readonly ITgsAgentService _agentService;
        private readonly ITgsBrandService _brandService;
        private readonly IPlayerAggregateService _playerAggregateService;

        public CreateAgentDividendsQueryHandler(
            IPlayerAggregateService playerAggregateService,
            ITgsAgentService agentService,
            ITgsBrandService brandService,
            IAgentDividendService agentDividendService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _brandService = brandService;
            _playerAggregateService = playerAggregateService;
            _agentDividendService = agentDividendService;
        }

        public override async Task<MessageResponse> Handle(CreateAgentDividendsRequest request,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));
            var brands = await _brandService.GetBrandQueries(q => q.Where(x => now.Date > x.BeforeToDateTime))
                .ToArrayAsync(cancellationToken);

            var createModels = new List<CreateAgentDividendModel>();

            foreach (var brand in brands)
            {
                var agents = await _agentService
                    .GetAgentQueries(q => q.Where(x => x.BrandId == brand.Id && x.ParentAgentId != null))
                    .GroupBy(x => new {x.Id, x.ParentAgentId},
                        (a, list) => new {a.Id, a.ParentAgentId})
                    .ToArrayAsync(cancellationToken);

                var agentBonus = await _playerAggregateService.GetPlayerAggregateQueries()
                    .Where(x => x.AgentId != null)
                    .Where(x => x.BrandId == brand.Id)
                    .Where(x => x.ReportDate >= brand.BeforeFromDateTime && x.ReportDate <= brand.BeforeToDateTime)
                    .GroupBy(x => new {x.BrandId, x.AgentId},
                        (x, pa) => new
                        {
                            x.AgentId,
                            x.BrandId,
                            DepositAmount = pa.Sum(p => p.DepositAmount),
                            WithdrawalAmount = pa.Sum(p => p.WithdrawalAmount),
                            AdjustmentAmount = pa.Sum(p => p.AdjustmentAmount),
                            BonusAmount = pa.Sum(p => p.BonusAmount),
                            BetAmount = pa.Sum(p => p.BetAmount),
                            Turnover = pa.Sum(p => p.Turnover),
                            WinLossAmount = pa.Sum(p => p.WinLossAmount),
                            NetProfit = pa.Sum(p => p.NetProfit)
                        }).GroupJoin(_playerAggregateService.Repository.PlayerGameTypeAggregates
                            .Where(x => x.AgentId != null)
                            .Where(x => x.BrandId == brand.Id)
                            .Where(x => x.ReportDate >= brand.BeforeFromDateTime &&
                                x.ReportDate <= brand.BeforeToDateTime),
                        p => new {p.BrandId, p.AgentId},
                        pg => new {pg.BrandId, pg.AgentId},
                        (p, pg) => new {p, pg})
                    .SelectMany(x => x.pg.DefaultIfEmpty(),
                        (x, pg) => new
                        {
                            x.p.AgentId,
                            x.p.BrandId,
                            x.p.DepositAmount,
                            x.p.WithdrawalAmount,
                            x.p.AdjustmentAmount,
                            x.p.BonusAmount,
                            x.p.BetAmount,
                            x.p.Turnover,
                            x.p.WinLossAmount,
                            x.p.NetProfit,
                            pg.AgentDividendPercentage,
                            pg
                        }).ToArrayAsync(cancellationToken);

                var agentDividends = agentBonus
                    .GroupBy(
                        x => new
                        {
                            x.AgentId,
                            x.BrandId,
                            x.AgentDividendPercentage,
                            x.DepositAmount,
                            x.WithdrawalAmount,
                            x.AdjustmentAmount,
                            x.BonusAmount,
                            x.BetAmount,
                            x.Turnover,
                            x.WinLossAmount,
                            x.NetProfit
                        },
                        (x, agents) =>
                        {
                            return new
                            {
                                x.AgentId,
                                x.BrandId,
                                x.AgentDividendPercentage,
                                x.DepositAmount,
                                x.WithdrawalAmount,
                                x.AdjustmentAmount,
                                x.BonusAmount,
                                x.BetAmount,
                                x.Turnover,
                                x.WinLossAmount,
                                x.NetProfit,
                                LiveDealerWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.LiveDealer).Sum(l => l.pg.WinLoss),
                                SlotWinLoss = agents.Where(l => l.pg.GameType == GameType.Slot).Sum(l => l.pg.WinLoss),
                                FishingWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.Fishing).Sum(l => l.pg.WinLoss),
                                ArcadeWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.Arcade).Sum(l => l.pg.WinLoss),
                                TableGameWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.TableGame).Sum(l => l.pg.WinLoss),
                                LotteryWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.Lottery).Sum(l => l.pg.WinLoss),
                                CardGameWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.CardGame).Sum(l => l.pg.WinLoss),
                                SportsWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.Sportsbook).Sum(l => l.pg.WinLoss),
                                ESportsWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.ESports).Sum(l => l.pg.WinLoss),
                                VirtualSportsWinLoss =
                                    agents.Where(l => l.pg.GameType == GameType.VirtualSports).Sum(l => l.pg.WinLoss),
                                WinLoss = agents.Sum(l => l.pg.WinLoss),
                                AgentDividendAmount = x.AgentDividendPercentage *
                                    (x.WinLossAmount - x.AdjustmentAmount -
                                        x.BonusAmount - agents
                                            .Sum(m => m.pg.VipRebateAmount)) / 100
                            };
                        })
                    .GroupBy(x => new {x.AgentId, x.BrandId, x.AgentDividendPercentage},
                        (x, models) => new CreateAgentDividendModel
                        {
                            AgentId = x.AgentId.Value,
                            BrandId = x.BrandId,
                            From = brand.BeforeFromDateTime,
                            To = brand.BeforeToDateTime,
                            AgentDividendPercentage = x.AgentDividendPercentage,
                            DepositAmount = models.Sum(m => m.DepositAmount),
                            WithdrawalAmount = models.Sum(m => m.WithdrawalAmount),
                            AdjustmentAmount = models.Sum(m => m.AdjustmentAmount),
                            BonusAmount = models.Sum(m => m.BonusAmount),
                            BetAmount = models.Sum(m => m.BetAmount),
                            Turnover = models.Sum(m => m.Turnover),
                            WinLossAmount = models.Sum(m => m.WinLossAmount),
                            NetProfit = models.Sum(m => m.NetProfit),
                            LiveDealerWinLoss = models.Sum(m => m.LiveDealerWinLoss),
                            SlotWinLoss = models.Sum(m => m.SlotWinLoss),
                            FishingWinLoss = models.Sum(m => m.FishingWinLoss),
                            ArcadeWinLoss = models.Sum(m => m.ArcadeWinLoss),
                            TableGameWinLoss = models.Sum(m => m.TableGameWinLoss),
                            LotteryWinLoss = models.Sum(m => m.LotteryWinLoss),
                            CardGameWinLoss = models.Sum(m => m.CardGameWinLoss),
                            SportsWinLoss = models.Sum(m => m.SportsWinLoss),
                            ESportsWinLoss = models.Sum(m => m.ESportsWinLoss),
                            VirtualSportsWinLoss = models.Sum(m => m.VirtualSportsWinLoss),
                            AgentDividendAmount = models.Sum(m => m.AgentDividendAmount) +
                                agents.Where(a => a.ParentAgentId == x.AgentId).Sum(child =>
                                {
                                    var childAgentPlayers = agentBonus
                                        .Where(a =>
                                            a.AgentId == child.Id).GroupBy(
                                            g => new
                                            {
                                                g.AgentId,
                                                g.AgentDividendPercentage,
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
                                                agent.AgentDividendPercentage,
                                                RebateAmount = childAgents.Sum(c => c.pg.VipRebateAmount)
                                            }).ToArray();

                                    return Math.Max(childAgentPlayers.Sum(c =>
                                            c.WinLossAmount - c.BonusAmount - c.AdjustmentAmount - c.RebateAmount) *
                                        (x.AgentDividendPercentage -
                                            childAgentPlayers.FirstOrDefault().AgentDividendPercentage) / 100, 0);
                                }),
                            CreatedOn = DateTimeOffset.Now
                        }).ToList();

                createModels.AddRange(agentDividends);

                var (beforeFromDate, beforeToDate, afterFromDate, afterToDate) =
                    BrandSettlementIntervalGenerator.GetInterval(brand.SettlementIntervalType);

                brand.BeforeFromDateTime = beforeFromDate;
                brand.BeforeToDateTime = beforeToDate;
                brand.AfterFromDateTime = afterFromDate;
                brand.AfterToDateTime = afterToDate;
            }

            await using var reportTransaction =
                await _agentDividendService.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await using var tgsTransaction =
                await _brandService.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                await _agentDividendService.CreateAgentDividendsAsync(createModels.ToArray());

                await _brandService.UpdateBrandsAsync(brands);

                await _brandService.CreateAgentTransactionsAsync(createModels.ToArray());

                await reportTransaction.CommitAsync(cancellationToken);
                await tgsTransaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await reportTransaction.RollbackAsync(cancellationToken);
                await tgsTransaction.RollbackAsync(cancellationToken);
                return CreateInvalidResponse<MessageResponse>(e.Message);
            }


            return CreateValidResponse<MessageResponse>();
        }
    }
}
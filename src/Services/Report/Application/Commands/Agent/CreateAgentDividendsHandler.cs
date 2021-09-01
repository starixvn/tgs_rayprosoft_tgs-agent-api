using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs.Models;
using Tgs.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Services.WalletService;

namespace TGS.Services.Report.Application.Commands.Agent
{
    public class
        CreateAgentDividendsHandler : Handler<CreateAgentDividendsRequest,
            MessageResponse>
    {
        private readonly IAgentDividendService _agentDividendService;
        private readonly ITgsAgentService _agentService;
        private readonly ITgsBrandService _brandService;
        private readonly IPlayerAggregateService _playerAggregateService;
        private readonly IAgentWalletRepository _walletRepository;

        public CreateAgentDividendsHandler(
            IPlayerAggregateService playerAggregateService,
            ITgsAgentService agentService,
            ITgsBrandService brandService,
            IAgentDividendService agentDividendService,
            IAgentWalletRepository walletRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _brandService = brandService;
            _playerAggregateService = playerAggregateService;
            _agentDividendService = agentDividendService;
            _walletRepository = walletRepository;
        }

        public override async Task<MessageResponse> Handle(CreateAgentDividendsRequest request,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));
            var queryDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, new(8, 0, 0));

            await SendAgentDividend(request.BrandCode, queryDate, cancellationToken);

            return CreateValidResponse<MessageResponse>();
        }

        private async Task<(List<CreateAgentDividendModel>, List<CreateAgentDividendModel>, List<AgentDividend>)>
            GetAgentDividends(Brand brand, int idNumber)
        {
            var agents = await _agentService
                .GetAgentQueries(q => q.Where(x => x.BrandId == brand.Id && x.ParentAgentId != null))
                .GroupBy(x => new
                    {
                        x.Id,
                        x.ParentAgentId
                    },
                    (a, list) => new
                    {
                        a.Id,
                        a.ParentAgentId
                    })
                .ToArrayAsync();

            var agentDividendGroup = await _playerAggregateService.GetPlayerAggregateQueries()
                .Where(x => x.AgentId != null)
                .Where(x => x.BrandId == brand.Id)
                .Where(x => x.ReportDate >= brand.CurrentSettlementFrom &&
                    x.ReportDate <= brand.CurrentSettlementTo)
                .GroupBy(x => new
                    {
                        x.BrandId,
                        x.AgentId,
                        x.AgentDividendPercentage
                    },
                    (groupItem, playerAggregates) => new
                    {
                        groupItem.AgentId,
                        groupItem.BrandId,
                        groupItem.AgentDividendPercentage,
                        DepositAmount = playerAggregates.Sum(p => p.DepositAmount),
                        WithdrawalAmount = playerAggregates.Sum(p => p.WithdrawalAmount),
                        AdjustmentAmount = playerAggregates.Sum(p => p.AdjustmentAmount),
                        BonusAmount = playerAggregates.Sum(p => p.RebateAmount + p.PromotionAmount),
                        BetAmount = playerAggregates.Sum(p => p.BetAmount),
                        Turnover = playerAggregates.Sum(p => p.Turnover),
                        WinLossAmount = playerAggregates.Sum(p => p.WinLossAmount),
                        NetProfit = playerAggregates.Sum(p
                            => p.WinLossAmount - p.AdjustmentAmount - p.RebateAmount - p.PromotionAmount)
                    }).GroupJoin(_playerAggregateService.Repository.PlayerGameAggregates.AsQueryable()
                        .Where(x => x.AgentId != null)
                        .Where(x => x.BrandId == brand.Id)
                        .Where(x => x.ReportDate >= brand.CurrentSettlementFrom &&
                            x.ReportDate <= brand.CurrentSettlementTo),
                    p => new
                    {
                        p.BrandId,
                        p.AgentId,
                        p.AgentDividendPercentage
                    },
                    pg => new
                    {
                        pg.BrandId,
                        pg.AgentId,
                        pg.AgentDividendPercentage
                    },
                    (agent, playerGameAggregates) => new
                    {
                        agent,
                        playerGameAggregates
                    })
                .SelectMany(x => x.playerGameAggregates.DefaultIfEmpty(),
                    (x, playerGameAggregate) => new
                    {
                        x.agent.AgentId,
                        x.agent.BrandId,
                        x.agent.AgentDividendPercentage,
                        x.agent.DepositAmount,
                        x.agent.WithdrawalAmount,
                        x.agent.AdjustmentAmount,
                        x.agent.BonusAmount,
                        x.agent.BetAmount,
                        x.agent.Turnover,
                        x.agent.WinLossAmount,
                        x.agent.NetProfit,
                        playerGameAggregate
                    }).ToArrayAsync();

            var agentDividends = agentDividendGroup
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
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.LiveDealer)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            SlotWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.Slot)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            FishingWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.Fishing)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            ArcadeWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.Arcade)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            TableGameWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.TableGame)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            LotteryWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.Lottery)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            CardGameWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.CardGame)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            SportsWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.Sportsbook)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            ESportsWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.ESports)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            VirtualSportsWinLoss =
                                agents
                                    .Where(l => l.playerGameAggregate != null)
                                    .Where(l => l.playerGameAggregate.GameType == GameType.VirtualSports)
                                    .Sum(l => l.playerGameAggregate.WinLoss),
                            WinLoss = agents
                                .Where(l => l.playerGameAggregate != null)
                                .Sum(l => l.playerGameAggregate.WinLoss),
                            AgentDividendAmount = x.AgentDividendPercentage * x.NetProfit / 100M
                        };
                    })
                .GroupBy(x => new
                    {
                        x.AgentId,
                        x.BrandId,
                        x.AgentDividendPercentage
                    },
                    (x, models) => new CreateAgentDividendModel
                    {
                        AgentId = x.AgentId.Value,
                        BrandId = x.BrandId,
                        From = brand.CurrentSettlementFrom,
                        To = brand.CurrentSettlementTo,
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
                                var childAgentPlayers = agentDividendGroup
                                    .Where(a =>
                                        a.AgentId == child.Id).GroupBy(
                                        g => new
                                        {
                                            g.AgentId,
                                            g.BrandId,
                                            g.AgentDividendPercentage,
                                            g.NetProfit
                                        },
                                        (agent, childAgents) => new
                                        {
                                            agent.AgentId,
                                            agent.NetProfit,
                                            agent.AgentDividendPercentage,
                                            RebateAmount = childAgents
                                                .Where(c => c.playerGameAggregate != null)
                                                .Sum(c =>
                                                    c.playerGameAggregate.VipRebateAmount)
                                        }).ToArray();

                                var amount = childAgentPlayers.Sum(c =>
                                    Math.Abs(x.AgentDividendPercentage - c.AgentDividendPercentage) * c.NetProfit /
                                    100M);
                                return amount;
                            }),
                        CreatedOn = DateTimeOffset.Now,
                        AutoSend = brand.AutoSentAgentBonus
                    }).ToArray();

            var agentIds = agentDividends.Where(x => x.AgentDividendAmount > 0M).Select(x => x.AgentId).ToArray();

            var noSendDividends = await _agentDividendService.GetAgentDividendQueries()
                .Where(x => agentIds.Contains(x.AgentId) && x.SentOn == null).ToArrayAsync().ConfigureAwait(false);

            foreach (var dividend in agentDividends)
            {
                dividend.Id = idNumber += 1;

                var agentNoSendDividends = noSendDividends.Where(x => x.AgentId == dividend.AgentId).ToArray();

                var agentNoSendDividend = agentNoSendDividends.Sum(x => x.AgentDividendAmount);

                if (agentNoSendDividend < 0)
                {
                    if (dividend.AgentDividendAmount + agentNoSendDividend > 0M)
                    {
                        dividend.BeforeDividendAmount = dividend.AgentDividendAmount;
                        dividend.AgentDividendAmount += agentNoSendDividend;

                        foreach (var negativeDividend in agentNoSendDividends)
                        {
                            negativeDividend.ReferencedAgentDividendId = dividend.Id;
                            negativeDividend.SentOn = DateTimeOffset.Now;
                        }
                    }
                    else
                    {
                        dividend.AutoSend = false;
                    }
                }
            }

            return (agentDividends.Where(x =>
                    x.AgentDividendAmount > 0 && x.AutoSend).ToList(),
                agentDividends.Where(x =>
                    x.AgentDividendAmount < 0 || !x.AutoSend).ToList(),
                noSendDividends.Where(x => x.ReferencedAgentDividendId is not null).ToList());
        }

        private static (DateTimeOffset, DateTimeOffset) GetInterval(
            SettlementIntervalType type, DateTimeOffset startDate, int? dayOfMonth)
        {
            switch (type)
            {
                case SettlementIntervalType.Daily:
                    return (startDate, startDate);
                case SettlementIntervalType.Weekly:
                    return (DateGenerator.GetFirstDayOfWeek(startDate),
                        DateGenerator.GetLastDayOfWeek(startDate));
                case SettlementIntervalType.Monthly:
                    return (startDate,
                        dayOfMonth == null
                            ? DateGenerator.GetLastDayOfMonth(startDate.AddMonths(1))
                            : DateGenerator.GetSpecifyDayOfMonth(startDate.AddMonths(1), dayOfMonth.Value!));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private async IAsyncEnumerable<Brand[]> GetBrands(string brandCode, DateTimeOffset queryDate,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            bool hasBrands;
            do
            {
                var brands = await _brandService.GetBrandQueries(q
                        =>
                        q.Where(brandCode is not null, x => x.Code == brandCode)
                            .Where(brandCode is null, x => queryDate > x.CurrentSettlementTo), true)
                    .ToArrayAsync(cancellationToken);
                hasBrands = brands.Any();
                yield return brands;
            } while (hasBrands);
        }

        private async Task SendAgentDividend(string brandCode, DateTimeOffset queryDate,
            CancellationToken cancellationToken)
        {
            await foreach (var brands in GetBrands(brandCode, queryDate, cancellationToken))
            {
                var createModels = new List<CreateAgentDividendModel>();
                var createAutoSendModels = new List<CreateAgentDividendModel>();
                var updateAgentDividends = new List<AgentDividend>();

                var maxNumber = await _agentDividendService.GetAgentDividendQueries()
                    .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;

                foreach (var brand in brands)
                {
                    if (!await _agentDividendService.GetAgentDividendQueries()
                        .AnyAsync(
                            x => x.BrandId == brand.Id && brand.CurrentSettlementFrom == x.From &&
                                brand.CurrentSettlementTo == x.To, cancellationToken))
                    {
                        var (autoSendDividends, manuelOrNegativeDividends, agentDividends) =
                            await GetAgentDividends(brand, maxNumber);

                        maxNumber += autoSendDividends.Count() + manuelOrNegativeDividends.Count();

                        createAutoSendModels.AddRange(autoSendDividends);
                        createModels.AddRange(manuelOrNegativeDividends);
                        updateAgentDividends.AddRange(agentDividends);
                    }

                    var (nextFrom, nextTo) =
                        GetInterval(brand.SettlementIntervalType,
                            brand.NextSettlementTo.AddDays(1), brand.SettlementDay);

                    brand.PreviousSettlementFrom = brand.CurrentSettlementFrom;
                    brand.PreviousSettlementTo = brand.CurrentSettlementTo;
                    brand.CurrentSettlementFrom = brand.NextSettlementFrom;
                    brand.CurrentSettlementTo = brand.NextSettlementTo;
                    brand.NextSettlementFrom = nextFrom;
                    brand.NextSettlementTo = nextTo;
                }

                await SaveAgentDividendData(brands, createModels, createAutoSendModels, updateAgentDividends,
                    cancellationToken);
            }
        }

        private async Task SaveAgentDividendData(Brand[] brands, List<CreateAgentDividendModel> createModels,
            List<CreateAgentDividendModel> createAutoSendModels, List<AgentDividend> updateAgentDividends,
            CancellationToken cancellationToken)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _agentDividendService.CreateAgentDividendsAsync(createModels.ToArray(), null);
            await _agentDividendService.UpdateAgentDividendsAsync(updateAgentDividends.ToArray());
            await _agentDividendService.CreateAgentDividendsAsync(
                createAutoSendModels.ToArray(),
                DateTimeOffset.Now);

            await _brandService.UpdateBrandsAsync(brands);

            var createdAgentIds = createAutoSendModels.Select(x => x.AgentId).ToArray();
            var createdAgents = await _agentService.GetAgentQueries().Where(x => createdAgentIds.Contains(x.Id))
                .ToArrayAsync(cancellationToken);

            foreach (var models in createAutoSendModels)
            {
                var agent = createdAgents.Single(a => a.Id == models.AgentId);
                await _walletRepository.ExchangeAsync(models.BrandId, agent.DefaultWalletId, agent.Id,
                    TransactionTypes.Dividend, models.AgentDividendAmount);
            }

            scope.Complete();
        }
    }
}
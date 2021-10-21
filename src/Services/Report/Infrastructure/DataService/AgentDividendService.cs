using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Report.Application.Models.Agent;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IAgentDividendService
    {
        ITgsRepository Repository { get; }

        IQueryable<AgentDividend> GetAgentDividendQueries(
            Func<IQueryable<AgentDividend>, IQueryable<AgentDividend>> preQuery = null,
            bool tracking = false);

        Task CreateAgentDividendsAsync(CreateAgentDividendModel[] request, DateTimeOffset? sentOn);
        Task UpdateAgentDividendsAsync(AgentDividend[] agentDividends);

        Task<(List<CreateAgentDividendModel>, List<CreateAgentDividendModel>, List<AgentDividend>)>
            GetAgentDividends(Brand brand, int idNumber);
    }

    public class AgentDividendService : BaseRepositoryOperations<ITgsRepository>, IAgentDividendService
    {
        private readonly ITgsRepository _repository;

        public AgentDividendService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IAgentDividendService.Repository => _repository;

        public async Task<(List<CreateAgentDividendModel>, List<CreateAgentDividendModel>, List<AgentDividend>)>
            GetAgentDividends(Brand brand, int idNumber)
        {
            var agents = await _repository.Agents.AsQueryable().Where(x => x.BrandId == brand.Id)
                .Include(x => x.AgentDividendConditions)
                .ToArrayAsync();

            var playerData = await _repository.PlayerAggregates.AsQueryable()
                .Where(x => x.AgentId != null)
                .Where(x => x.BrandId == brand.Id)
                .Where(x => x.ReportDate >= brand.CurrentSettlementFrom &&
                    x.ReportDate <= brand.CurrentSettlementTo)
                .GroupBy(x => new
                    {
                        x.BrandId,
                        x.AgentId,
                        x.PlayerCode
                    },
                    (groupItem, playerAggregates) => new
                    {
                        groupItem.AgentId,
                        groupItem.PlayerCode,
                        groupItem.BrandId,
                        PromotionAmount = playerAggregates.Sum(p => p.PromotionAmount),
                        RebateAmount = playerAggregates.Sum(p => p.RebateAmount),
                        DepositAmount = playerAggregates.Sum(p => p.DepositAmount),
                        WithdrawalAmount = playerAggregates.Sum(p => p.WithdrawalAmount),
                        AdjustmentAmount = playerAggregates.Sum(p => p.AdjustmentAmount),
                        BonusAmount = playerAggregates.Sum(p => p.RebateAmount + p.PromotionAmount),
                        BetAmount = playerAggregates.Sum(p => p.BetAmount),
                        Turnover = playerAggregates.Sum(p => p.Turnover),
                        WinLossAmount = playerAggregates.Sum(p => p.WinLossAmount),
                        NetProfit = playerAggregates.Sum(p
                            => p.WinLossAmount - p.AdjustmentAmount - p.RebateAmount - p.PromotionAmount -
                            p.WinLossAmount * brand.PlatformCharge / 100)
                    })
                .Select(x => new
                {
                    AgentId = x.AgentId.Value,
                    x.PlayerCode,
                    x.BrandId,
                    x.PromotionAmount,
                    x.RebateAmount,
                    x.DepositAmount,
                    x.WithdrawalAmount,
                    x.AdjustmentAmount,
                    x.BonusAmount,
                    x.BetAmount,
                    x.Turnover,
                    x.WinLossAmount,
                    x.NetProfit
                }).ToArrayAsync();

            var parentAgentDividends = playerData.SelectMany(x => GetAgentDividends(agents,
                brand.Id,
                x.AgentId,
                x.NetProfit)).ToArray();

            var agentData = playerData.GroupBy(x => new
                {
                    x.BrandId,
                    x.AgentId
                },
                (groupItem, playerAggregates) => new
                {
                    groupItem.AgentId,
                    groupItem.BrandId,
                    PromotionAmount = playerAggregates.Sum(p => p.PromotionAmount),
                    RebateAmount = playerAggregates.Sum(p => p.RebateAmount),
                    DepositAmount = playerAggregates.Sum(p => p.DepositAmount),
                    WithdrawalAmount = playerAggregates.Sum(p => p.WithdrawalAmount),
                    AdjustmentAmount = playerAggregates.Sum(p => p.AdjustmentAmount),
                    BonusAmount = playerAggregates.Sum(p => p.BonusAmount),
                    BetAmount = playerAggregates.Sum(p => p.BetAmount),
                    Turnover = playerAggregates.Sum(p => p.Turnover),
                    WinLossAmount = playerAggregates.Sum(p => p.WinLossAmount),
                    NetProfit = playerAggregates.Sum(p => p.NetProfit)
                });

            var allAgents = agents.GroupJoin(agentData,
                agent => new
                {
                    agent.Id,
                    agent.BrandId
                },
                data => new
                {
                    Id = data.AgentId,
                    data.BrandId
                },
                (agent, data) => new
                {
                    agent,
                    data
                }).SelectMany(x => x.data.DefaultIfEmpty(),
                (x, data) => new
                {
                    AgentId = x.agent.Id,
                    x.agent.Username,
                    x.agent.BrandId,
                    x.agent.AgentDividendConditions.First(c => c.Rank == 1).DividendPercentage,
                    data?.PromotionAmount,
                    data?.RebateAmount,
                    data?.DepositAmount,
                    data?.WithdrawalAmount,
                    data?.AdjustmentAmount,
                    data?.BonusAmount,
                    data?.BetAmount,
                    data?.Turnover,
                    data?.WinLossAmount,
                    data?.NetProfit
                });

            var agentGameData = _repository.PlayerGameAggregates.AsQueryable()
                .Where(x => x.AgentId != null)
                .Where(x => x.BrandId == brand.Id)
                .Where(x => x.ReportDate >= brand.CurrentSettlementFrom &&
                    x.ReportDate <= brand.CurrentSettlementTo).GroupBy(x => new
                    {
                        x.BrandId,
                        x.AgentId
                    },
                    (agent, playerGames) => new
                    {
                        AgentId = agent.AgentId.Value,
                        agent.BrandId,
                        LiveDealerWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.LiveDealer)
                                .Sum(l => l.WinLoss),
                        SlotWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.Slot)
                                .Sum(l => l.WinLoss),
                        FishingWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.Fishing)
                                .Sum(l => l.WinLoss),
                        ArcadeWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.Arcade)
                                .Sum(l => l.WinLoss),
                        TableGameWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.TableGame)
                                .Sum(l => l.WinLoss),
                        LotteryWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.Lottery)
                                .Sum(l => l.WinLoss),
                        CardGameWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.CardGame)
                                .Sum(l => l.WinLoss),
                        SportsWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.Sportsbook)
                                .Sum(l => l.WinLoss),
                        ESportsWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.ESports)
                                .Sum(l => l.WinLoss),
                        VirtualSportsWinLoss =
                            playerGames
                                .Where(l => l.GameType == GameType.VirtualSports)
                                .Sum(l => l.WinLoss),
                        WinLoss = playerGames
                            .Sum(l => l.WinLoss)
                    });

            var agentDividends = allAgents.GroupJoin(agentGameData,
                    agent => new
                    {
                        agent.BrandId,
                        agent.AgentId
                    },
                    agentGame => new
                    {
                        agentGame.BrandId,
                        agentGame.AgentId
                    },
                    (agent, games) => new
                    {
                        agent,
                        games
                    })
                .SelectMany(x => x.games.DefaultIfEmpty(),
                    (x, games) => new CreateAgentDividendModel
                    {
                        AgentUsername = x.agent.Username,
                        AgentId = x.agent.AgentId,
                        BrandId = x.agent.BrandId,
                        AutoSend = brand.AutoSentAgentBonus,
                        From = brand.CurrentSettlementFrom,
                        To = brand.CurrentSettlementTo,
                        PromotionAmount = x.agent?.PromotionAmount ?? 0M,
                        RebateAmount = x.agent?.RebateAmount ?? 0M,
                        DepositAmount = x.agent?.DepositAmount ?? 0M,
                        WithdrawalAmount = x.agent?.WithdrawalAmount ?? 0M,
                        AdjustmentAmount = x.agent?.AdjustmentAmount ?? 0M,
                        BonusAmount = x.agent?.BonusAmount ?? 0M,
                        BetAmount = x.agent?.BetAmount ?? 0M,
                        Turnover = x.agent?.Turnover ?? 0M,
                        WinLossAmount = x.agent?.WinLossAmount ?? 0M,
                        NetProfit = x.agent?.NetProfit ?? 0M,
                        LiveDealerWinLoss = games?.LiveDealerWinLoss ?? 0M,
                        SlotWinLoss = games?.SlotWinLoss ?? 0M,
                        FishingWinLoss = games?.FishingWinLoss ?? 0M,
                        ArcadeWinLoss = games?.ArcadeWinLoss ?? 0M,
                        TableGameWinLoss = games?.TableGameWinLoss ?? 0M,
                        LotteryWinLoss = games?.LotteryWinLoss ?? 0M,
                        CardGameWinLoss = games?.CardGameWinLoss ?? 0M,
                        SportsWinLoss = games?.SportsWinLoss ?? 0M,
                        ESportsWinLoss = games?.ESportsWinLoss ?? 0M,
                        VirtualSportsWinLoss = games?.VirtualSportsWinLoss ?? 0M,
                        AgentDividendPercentage = x.agent?.DividendPercentage ?? 0M,
                        AgentDividendAmount =
                            parentAgentDividends.Where(p => p.AgentId == x.agent.AgentId).Sum(s => s.Dividend),
                        AgentDividendMultipleLayerModels =
                            parentAgentDividends.Where(p => p.AgentId == x.agent.AgentId).ToArray(),
                        CreatedOn = DateTimeOffset.Now
                    }).Where(x => x.AgentDividendAmount != 0M).ToArray();

            var agentIds = agentDividends.Where(x => x.AgentDividendAmount > 0M).Select(x => x.AgentId).ToArray();

            var noSendDividends = await _repository.AgentDividends.AsQueryable()
                .Where(x => agentIds.Contains(x.AgentId) && x.SentOn == null).ToArrayAsync()
                .ConfigureAwait(false);

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

        Task IAgentDividendService.
            CreateAgentDividendsAsync(CreateAgentDividendModel[] models, DateTimeOffset? sentOn) =>
            CreateEntitiesAsync(models.Select(model => new AgentDividend
            {
                Id = model.Id.Value,
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                From = model.From,
                To = model.To,
                AgentDividendPercentage = model.AgentDividendPercentage,
                PromotionAmount = model.PromotionAmount,
                RebateAmount = model.RebateAmount,
                DepositAmount = model.DepositAmount,
                WithdrawalAmount = model.WithdrawalAmount,
                AdjustmentAmount = model.AdjustmentAmount,
                BonusAmount = model.BonusAmount,
                BetAmount = model.BetAmount,
                Turnover = model.Turnover,
                WinLossAmount = model.WinLossAmount,
                NetProfit = model.NetProfit,
                LiveDealerWinLoss = model.LiveDealerWinLoss,
                SlotWinLoss = model.SlotWinLoss,
                FishingWinLoss = model.FishingWinLoss,
                ArcadeWinLoss = model.ArcadeWinLoss,
                TableGameWinLoss = model.TableGameWinLoss,
                LotteryWinLoss = model.LotteryWinLoss,
                CardGameWinLoss = model.CardGameWinLoss,
                SportsWinLoss = model.SportsWinLoss,
                ESportsWinLoss = model.ESportsWinLoss,
                VirtualSportsWinLoss = model.VirtualSportsWinLoss,
                AgentDividendAmount = model.AgentDividendAmount,
                CreatedOn = model.CreatedOn,
                SentOn = sentOn,
                BeforeDividendAmount = model.BeforeDividendAmount,
                ReferencedAgentDividendId = model.ReferencedAgentDividendId,
                AgentDividendMultipleLayers = model.AgentDividendMultipleLayerModels.Select(x
                    => new AgentDividendMultipleLayer
                    {
                        AgentId = x.AgentId,
                        AgentDividendId = model.Id.Value,
                        DividendProviderAgentId = x.DividendProviderAgentId,
                        Layer = x.Layer,
                        Dividend = x.Dividend
                    }).ToArray()
            }).ToArray());

        IQueryable<AgentDividend> IAgentDividendService.GetAgentDividendQueries(
            Func<IQueryable<AgentDividend>, IQueryable<AgentDividend>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task IAgentDividendService.UpdateAgentDividendsAsync(AgentDividend[] agentDividends) =>
            UpdateEntitiesAsync(agentDividends);

        private static IEnumerable<AgentDividendMultipleLayerModel> GetAgentDividends(
            Agent[] agents, Guid brandId, Guid agentId,
            decimal netProfit)
        {
            bool hasParentAgentId;

            var agent = agents.First(x => x.Id == agentId);
            var currentAgentId = agentId;
            var providerAgentId = agent.Id;
            var childDividendPercentage = 0M;
            var layer = agent.Id.ToString();

            do
            {
                var agentDividendPercentage = agent.AgentDividendConditions.First(x => x.Rank == 1).DividendPercentage;

                yield return new AgentDividendMultipleLayerModel
                {
                    AgentId = currentAgentId,
                    DividendProviderAgentId = providerAgentId,
                    Layer = layer,
                    Dividend = netProfit * (agentDividendPercentage -
                        childDividendPercentage) / 100M
                };

                hasParentAgentId = agent.ParentAgentId != null;

                if (!hasParentAgentId)
                {
                    continue;
                }

                childDividendPercentage = agentDividendPercentage;

                var parentAgent = agents.First(x => x.Id == agent.ParentAgentId);
                currentAgentId = parentAgent.Id;

                layer = $"{currentAgentId},{layer}";

                agent = parentAgent;
            } while (hasParentAgentId);
        }
    }
}
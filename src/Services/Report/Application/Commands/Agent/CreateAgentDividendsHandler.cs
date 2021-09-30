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
        private readonly IPlayerGameAggregateService _playerGameAggregateService;
        private readonly IAgentWalletRepository _walletRepository;

        public CreateAgentDividendsHandler(
            IPlayerAggregateService playerAggregateService,
            IPlayerGameAggregateService playerGameAggregateService,
            ITgsAgentService agentService,
            ITgsBrandService brandService,
            IAgentDividendService agentDividendService,
            IAgentWalletRepository walletRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _brandService = brandService;
            _playerAggregateService = playerAggregateService;
            _playerGameAggregateService = playerGameAggregateService;
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

        private static IEnumerable<AgentDividendMultipleLayerModel> GetAgentDividends(
            Core.DataServices.Tgs.Models.Agent[] agents, Guid brandId, Guid agentId,
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
                yield return new AgentDividendMultipleLayerModel
                {
                    AgentId = currentAgentId,
                    DividendProviderAgentId = providerAgentId,
                    Layer = layer,
                    Dividend = netProfit * (agent.DividendPercentage - childDividendPercentage) / 100M
                };

                hasParentAgentId = agent.ParentAgentId != null;

                if (!hasParentAgentId)
                {
                    continue;
                }

                childDividendPercentage = agent.DividendPercentage;

                var parentAgent = agents.First(x => x.Id == agent.ParentAgentId);
                currentAgentId = parentAgent.Id;

                layer = $"{currentAgentId},{layer}";

                agent = parentAgent;
            } while (hasParentAgentId);
        }

        private async Task<(List<CreateAgentDividendModel>, List<CreateAgentDividendModel>, List<AgentDividend>)>
            GetAgentDividends(Brand brand, int idNumber, CancellationToken cancellationToken)
        {
            var agents = await _agentService
                .GetAgentQueries(q => q.Where(x => x.BrandId == brand.Id))
                .ToArrayAsync(cancellationToken);

            var playerData = await _playerAggregateService.GetPlayerAggregateQueries()
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
                            p.WinLossAmount * brand.PlatformCharge)
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
                }).ToArrayAsync(cancellationToken);

            var parentAgentDividends = playerData.SelectMany(x => GetAgentDividends(agents, brand.Id, x.AgentId,
                x.NetProfit)).ToArray();

            var agentData = playerData.GroupBy(x => new
            {
                x.BrandId,
                x.AgentId
            }, (groupItem, playerAggregates) => new
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

            var allAgents = agents.GroupJoin(agentData, agent => new
                {
                    agent.Id,
                    agent.BrandId
                }, data => new
                {
                    Id = data.AgentId,
                    data.BrandId
                },
                (agent, data) => new
                {
                    agent,
                    data
                }).SelectMany(x => x.data.DefaultIfEmpty(), (x, data) => new
            {
                AgentId = x.agent.Id,
                x.agent.BrandId,
                x.agent.DividendPercentage,
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

            var agentGameData = _playerGameAggregateService.GetPlayerGameAggregateQueries()
                .Where(x => x.AgentId != null)
                .Where(x => x.BrandId == brand.Id)
                .Where(x => x.ReportDate >= brand.CurrentSettlementFrom &&
                    x.ReportDate <= brand.CurrentSettlementTo).GroupBy(x => new
                {
                    x.BrandId,
                    x.AgentId
                }, (agent, playerGames) => new
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

            var noSendDividends = await _agentDividendService.GetAgentDividendQueries()
                .Where(x => agentIds.Contains(x.AgentId) && x.SentOn == null).ToArrayAsync(cancellationToken)
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

        private static (DateTimeOffset, DateTimeOffset) GetInterval(
            SettlementIntervalType type, DateTimeOffset startDate, int? dayOfMonth) => type switch
        {
            SettlementIntervalType.Daily => (startDate, startDate),
            SettlementIntervalType.Weekly => (DateGenerator.GetFirstDayOfWeek(startDate),
                DateGenerator.GetLastDayOfWeek(startDate)),
            SettlementIntervalType.Monthly => (startDate,
                dayOfMonth == null
                    ? DateGenerator.GetLastDayOfMonth(startDate.AddMonths(1))
                    : DateGenerator.GetSpecifyDayOfMonth(startDate.AddMonths(1), dayOfMonth.Value!)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

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
                            await GetAgentDividends(brand, maxNumber, cancellationToken);

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
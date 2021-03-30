using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.Report.Application.Models.Market;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Application.Models.Round;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IRoundAggregateService
    {
        IQueryable<RoundAggregate> GetRoundAggregateQueries(
            Func<IQueryable<RoundAggregate>, IQueryable<RoundAggregate>> preQuery = null,
            bool tracking = false);

        Task CreateRoundAggregateAsync(RoundAggregateModel[] models);
        Task DeleteRoundAggregatesAsync(RoundAggregate[] roundAggregates);
        Task<bool> ExistsAsync(Expression<Func<RoundAggregate, bool>> predicate);
        Task<MarketDataModel[]> GetMarketsAsync(DateTimeOffset start, DateTimeOffset end);
        Task<PlayerDataModel[]> GetPlayersAsync(DateTimeOffset start, DateTimeOffset end);

        Task<(PlayerAggregateModel[] playerAggregates, PlayerGameTypeAggregateModel[] playerAggregateByGameType)>
            GetPlayerAggregatesAsync(DateTimeOffset start,
                DateTimeOffset end);
    }

    public class RoundAggregateService : BaseRepositoryOperations<IReportRepository>, IRoundAggregateService
    {
        public readonly IReportRepository _reportRepository;
        public readonly ITgsRepository _tgsRepository;

        public RoundAggregateService(IReportRepository reportRepository, ITgsRepository tgsRepository) : base(
            reportRepository)
        {
            _tgsRepository = tgsRepository;
            _reportRepository = reportRepository;
        }

        Task IRoundAggregateService.CreateRoundAggregateAsync(RoundAggregateModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new RoundAggregate
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                PlayerCode = model.PlayerCode,
                GameType = model.GameType,
                Hour = model.Hour,
                BetAmount = model.BetAmount,
                ValidBetAmount = model.ValidBetAmount,
                Turnover = model.Turnover,
                WinLoss = model.WinLoss,
                ReportDate = model.ReportDate,
                CreatedOn = model.CreatedOn
            }).ToArray());

        Task IRoundAggregateService.DeleteRoundAggregatesAsync(RoundAggregate[] roundAggregates) =>
            DeleteEntitiesAsync(roundAggregates);

        Task<bool> IRoundAggregateService.ExistsAsync(Expression<Func<RoundAggregate, bool>> predicate) =>
            ExistsAsync(predicate);

        IQueryable<RoundAggregate> IRoundAggregateService.GetRoundAggregateQueries(
            Func<IQueryable<RoundAggregate>, IQueryable<RoundAggregate>> preQuery, bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task<MarketDataModel[]> IRoundAggregateService.
            GetMarketsAsync(DateTimeOffset start, DateTimeOffset end)
        {
            var reportDate = new DateTimeOffset(start.Year, start.Month, start.Day, 0,
                0, 0,
                start.Offset);

            var roundAggregates =
                await GetQueriesAsync<RoundAggregate>(
                    q => q.Where(x => x.ReportDate == reportDate && x.Hour == start.Hour && x.Minute == start.Minute),
                    false).ToArrayAsync();

            var markets = await _tgsRepository.Brands
                .Select(x =>
                    new MarketDataModel
                    {
                        BrandId = x.Id,
                        Hour = start.Hour,
                        ReportDate = reportDate,
                        RegisteredCount =
                            _tgsRepository.Players.Count(p =>
                                p.BrandId == x.Id && p.RegisteredOn >= start && p.RegisteredOn < end),
                        FirstFundInCount =
                            _tgsRepository.Players.Count(p =>
                                p.BrandId == x.Id && p.FirstDepositOn >= start && p.FirstDepositOn < end),
                        DepositAmount =
                            _tgsRepository.Transactions.Where(t =>
                                t.BrandId == x.Id && t.Type == TransactionType.Deposit && t.ConfirmedOn >= start &&
                                t.ConfirmedOn < end).Sum(t => t.Amount),
                        WithdrawalAmount =
                            _tgsRepository.Transactions.Where(t =>
                                t.BrandId == x.Id && t.Type == TransactionType.Withdrawal && t.ConfirmedOn >= start &&
                                t.ConfirmedOn < end).Sum(t => t.Amount),
                        AdjustmentAmount = _tgsRepository.Transactions.Where(t =>
                            t.BrandId == x.Id && t.Type == TransactionType.Adjust && t.ConfirmedOn >= start &&
                            t.ConfirmedOn < end).Sum(t => t.Amount),
                        CreatedOn = DateTimeOffset.Now
                    }).ToArrayAsync();

            foreach (var market in markets)
            {
                market.BetAmount =
                    roundAggregates.Where(r => r.BrandId == market.BrandId)
                        .Sum(r => r.BetAmount);
                market.ValidBetAmount =
                    roundAggregates.Where(r => r.BrandId == market.BrandId)
                        .Sum(r => r.Turnover);
                market.WinLossAmount = roundAggregates
                    .Where(r => r.BrandId == market.BrandId)
                    .Sum(r => r.WinLoss);
                market.NetProfit = market.WinLossAmount - market.AdjustmentAmount - market.BonusAmount;
            }

            return markets;
        }

        async Task<PlayerDataModel[]> IRoundAggregateService.GetPlayersAsync(DateTimeOffset start, DateTimeOffset end)
        {
            var reportDate = new DateTimeOffset(start.Year, start.Month, start.Day, 0,
                0, 0,
                start.Offset);

            var brandIds = await _tgsRepository.Brands.Select(x => x.Id).ToArrayAsync();

            return brandIds.Join(GetQueriesAsync<RoundAggregate>(
                    q => q.Where(x => x.ReportDate == reportDate && x.Hour == start.Hour && x.Minute == start.Minute),
                    false), brandId => brandId, ra => ra.BrandId, (brandId, ra) => new {brandId, ra})
                .GroupBy(x => new
                    {
                        x.brandId,
                        x.ra.AgentId,
                        x.ra.PlayerCode,
                        x.ra.ReportDate,
                        x.ra.Hour,
                        x.ra.Minute
                    },
                    (x, ra) => new PlayerDataModel
                    {
                        BrandId = x.brandId,
                        AgentId = x.AgentId,
                        PlayerCode = x.PlayerCode,
                        Hour = x.Hour,
                        Contribution = ra.Sum(r => r.ra.WinLoss),
                        ReportDate = x.ReportDate,
                        CreatedOn = DateTimeOffset.Now
                    }).ToArray();
        }

        async Task<(PlayerAggregateModel[] playerAggregates, PlayerGameTypeAggregateModel[] playerAggregateByGameType)
            > IRoundAggregateService.
            GetPlayerAggregatesAsync(DateTimeOffset start, DateTimeOffset end)
        {
            var reportDate = new DateTimeOffset(start.Year, start.Month, start.Day, 0,
                0, 0,
                start.Offset);

            var roundAggregates =
                await GetQueriesAsync<RoundAggregate>(
                    q => q.Where(x => x.ReportDate == reportDate && x.Hour == start.Hour && x.Minute == start.Minute),
                    false).ToArrayAsync();

            var playerAggregates = await _tgsRepository.Players
                .GroupJoin(_tgsRepository.Transactions, p => p.PlayerCode, t => t.Wallet.PlayerCode,
                    (p, t) => new {p.BrandId, player = p, transaction = t})
                .SelectMany(x => x.transaction.DefaultIfEmpty(),
                    (x, transaction) => new {x.BrandId, x.player, transaction})
                .GroupBy(x => new {x.BrandId, x.player.AgentId, x.player.PlayerCode}, (x, players) =>
                    new PlayerAggregateModel
                    {
                        BrandId = x.BrandId,
                        Hour = start.Hour,
                        ReportDate = reportDate,
                        AgentId = x.AgentId,
                        PlayerCode = x.PlayerCode,
                        RegisteredCount =
                            players.Count(p =>
                                p.player.RegisteredOn >= start && p.player.RegisteredOn < end),
                        FirstFundInCount =
                            players.Count(p =>
                                p.player.FirstDepositOn >= start &&
                                p.player.FirstDepositOn < end),
                        DepositAmount =
                            players.Where(p =>
                                p.transaction.Type == TransactionType.Deposit && p.transaction.ConfirmedOn >= start &&
                                p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                        WithdrawalAmount =
                            players.Where(p =>
                                p.transaction.Type == TransactionType.Withdrawal &&
                                p.transaction.ConfirmedOn >= start &&
                                p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                        AdjustmentAmount = players.Where(p =>
                            p.transaction.Type == TransactionType.Adjust && p.transaction.ConfirmedOn >= start &&
                            p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                        CreatedOn = DateTimeOffset.Now
                    }).ToArrayAsync();

            foreach (var playerAggregate in playerAggregates)
            {
                playerAggregate.BetAmount =
                    roundAggregates.Where(r => r.PlayerCode == playerAggregate.PlayerCode)
                        .Sum(r => r.BetAmount);
                playerAggregate.ValidBetAmount =
                    roundAggregates.Where(r => r.PlayerCode == playerAggregate.PlayerCode)
                        .Sum(r => r.Turnover);
                playerAggregate.WinLossAmount = roundAggregates
                    .Where(r => r.PlayerCode == playerAggregate.PlayerCode)
                    .Sum(r => r.WinLoss);
                playerAggregate.NetProfit = playerAggregate.WinLossAmount - playerAggregate.AdjustmentAmount -
                    playerAggregate.BonusAmount;
            }

            playerAggregates = playerAggregates.Where(x =>
                x.RegisteredCount > 0 || x.FirstFundInCount > 0 || x.DepositAmount > 0 || x.WithdrawalAmount > 0 ||
                x.AdjustmentAmount != 0 || x.BonusAmount > 0 || x.BetAmount > 0 || x.ValidBetAmount > 0 ||
                x.WinLossAmount != 0 || x.NetProfit != 0).ToArray();

            var playerAggregateByGameType = roundAggregates.Join(_tgsRepository.Players.Include(x => x.Vip),
                    r => r.PlayerCode, p => p.PlayerCode, (r, p) => new {r, p})
                .GroupBy(x => new
                    {
                        x.r.BrandId,
                        x.r.AgentId,
                        x.r.PlayerCode,
                        x.r.GameType,
                        x.p.Vip
                    },
                    (x, aggregate) =>
                    {
                        var liveDealerValidBetAmount = aggregate.Where(a => a.r.GameType == GameType.LiveDealer)
                            .Sum(a => a.r.ValidBetAmount);
                        var slotValidBetAmount = aggregate.Where(a => a.r.GameType == GameType.Slot)
                            .Sum(a => a.r.ValidBetAmount);
                        var cardGameValidBetAmount = aggregate.Where(a => a.r.GameType == GameType.CardGame)
                            .Sum(a => a.r.ValidBetAmount);
                        var sportsValidBetAmount = aggregate.Where(a => a.r.GameType == GameType.Sportsbook)
                            .Sum(a => a.r.ValidBetAmount);
                        var virtualSportsValidBetAmount = aggregate.Where(a => a.r.GameType == GameType.VirtualSports)
                            .Sum(a => a.r.ValidBetAmount);
                        return new PlayerGameTypeAggregateModel
                        {
                            BrandId = x.BrandId,
                            AgentId = x.AgentId,
                            PlayerCode = x.PlayerCode,
                            GameType = x.GameType,
                            Hour = start.Hour,
                            LiveDealerRebate = x.Vip.LiveDealerRebate,
                            LiveDealerValidBetAmount = liveDealerValidBetAmount,
                            LiveDealerAmount =
                                x.Vip.LiveDealerRebate * liveDealerValidBetAmount / 100,
                            SlotRebate = x.Vip.SlotRebate,
                            SlotValidBetAmount = slotValidBetAmount,
                            SlotAmount =
                                x.Vip.SlotRebate * slotValidBetAmount / 100,
                            CardGameRebate = x.Vip.CardGameRebate,
                            CardGameValidBetAmount = cardGameValidBetAmount,
                            CardGameAmount =
                                x.Vip.CardGameRebate * cardGameValidBetAmount / 100,
                            SportsRebate = x.Vip.SportsRebate,
                            SportsValidBetAmount = sportsValidBetAmount,
                            SportsAmount =
                                x.Vip.SportsRebate * sportsValidBetAmount / 100,
                            VirtualSportsRebate = x.Vip.CardGameRebate,
                            VirtualSportsValidBetAmount = virtualSportsValidBetAmount,
                            VirtualSportsAmount =
                                x.Vip.VirtualSportsRebate * virtualSportsValidBetAmount / 100,
                            CreatedOn = DateTimeOffset.Now,
                            ReportDate = reportDate
                        };
                    }).ToArray();

            return (playerAggregates, playerAggregateByGameType);
        }
    }
}
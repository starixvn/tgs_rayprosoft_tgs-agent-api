using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Report;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Application.Models.Round;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IRoundAggregateService
    {
        IReportRepository Repository { get; }

        IQueryable<RoundAggregate> GetRoundAggregateQueries(
            Func<IQueryable<RoundAggregate>, IQueryable<RoundAggregate>> preQuery = null,
            bool tracking = false);

        Task CreateRoundAggregateAsync(RoundAggregateModel[] models);
        Task DeleteRoundAggregatesAsync(RoundAggregate[] roundAggregates);
        Task<bool> ExistsAsync(Expression<Func<RoundAggregate, bool>> predicate);

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

        IReportRepository IRoundAggregateService.Repository => _reportRepository;

        Task IRoundAggregateService.CreateRoundAggregateAsync(RoundAggregateModel[] models) =>
            CreateEntitiesAsync(models.Select(model => new RoundAggregate
            {
                BrandId = model.BrandId,
                AgentId = model.AgentId,
                GameProviderId = model.GameProviderId,
                PlayerCode = model.PlayerCode,
                GameType = model.GameType,
                Hour = model.Hour,
                Minute = model.Minute,
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

            var playerAggregates = await _tgsRepository.Players.Include(x => x.Agent)
                .GroupJoin(_tgsRepository.Transactions, p => p.PlayerCode, t => t.Wallet.PlayerCode,
                    (p, t) => new {p.BrandId, Player = p, transaction = t})
                .SelectMany(x => x.transaction.DefaultIfEmpty(),
                    (x, transaction) => new {x.BrandId, x.Player, transaction})
                .GroupBy(x => new {x.BrandId, x.Player.AgentId, x.Player.PlayerCode},
                    (x, players) =>
                        new PlayerAggregateModel
                        {
                            BrandId = x.BrandId,
                            Hour = start.Hour,
                            ReportDate = reportDate,
                            AgentId = x.AgentId,
                            PlayerCode = x.PlayerCode,
                            RegisteredCount =
                                players.Count(p =>
                                    p.Player.RegisteredOn >= start && p.Player.RegisteredOn < end),
                            FirstFundInCount =
                                players.Count(p =>
                                    p.Player.FirstDepositOn >= start &&
                                    p.Player.FirstDepositOn < end),
                            DepositAmount =
                                players.Where(p =>
                                    p.transaction.Type == TransactionType.Deposit &&
                                    p.transaction.ConfirmedOn >= start &&
                                    p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                            WithdrawalAmount =
                                players.Where(p =>
                                    p.transaction.Type == TransactionType.Withdrawal &&
                                    p.transaction.ConfirmedOn >= start &&
                                    p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                            AdjustmentAmount = players.Where(p =>
                                p.transaction.Type == TransactionType.Adjust && p.transaction.ConfirmedOn >= start &&
                                p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                            BonusAmount = players.Where(p =>
                                p.transaction.Type == TransactionType.Bonus && p.transaction.ConfirmedOn >= start &&
                                p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                            RebateAmount = players.Where(p =>
                                p.transaction.Type == TransactionType.Rebate && p.transaction.ConfirmedOn >= start &&
                                p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                            DividendAmount = players.Where(p =>
                                p.transaction.Type == TransactionType.Dividend && p.transaction.ConfirmedOn >= start &&
                                p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                            PromotionAmount = players.Where(p =>
                                p.transaction.Type == TransactionType.Promotion && p.transaction.ConfirmedOn >= start &&
                                p.transaction.ConfirmedOn < end).Sum(t => t.transaction.Amount),
                            CreatedOn = DateTimeOffset.Now
                        }).ToArrayAsync();

            foreach (var playerAggregate in playerAggregates)
            {
                var playerRoundAggregates =
                    roundAggregates.Where(r => r.PlayerCode == playerAggregate.PlayerCode).ToArray();

                playerAggregate.BetAmount =
                    playerRoundAggregates
                        .Sum(r => r.BetAmount);
                playerAggregate.Turnover =
                    playerRoundAggregates
                        .Sum(r => r.Turnover);
                playerAggregate.WinLossAmount = playerRoundAggregates
                    .Sum(r => r.WinLoss);
                playerAggregate.NetProfit = playerAggregate.WinLossAmount - playerAggregate.AdjustmentAmount -
                    playerAggregate.BonusAmount - playerAggregate.RebateAmount - playerAggregate.DividendAmount -
                    playerAggregate.PromotionAmount;
            }

            playerAggregates = playerAggregates.Where(x =>
                x.RegisteredCount > 0 || x.FirstFundInCount > 0 || x.DepositAmount > 0 || x.WithdrawalAmount > 0 ||
                x.AdjustmentAmount != 0 || x.BonusAmount > 0 || x.BetAmount > 0 || x.Turnover > 0 ||
                x.WinLossAmount != 0 || x.NetProfit != 0).ToArray();

            var playerAggregateByGameType = roundAggregates.GroupJoin(
                    _tgsRepository.Players.Include(x => x.Agent).Include(x => x.Vip),
                    r => r.PlayerCode, p => p.PlayerCode, (r, p) => new {r, p = p.DefaultIfEmpty()})
                .SelectMany(x => x.p.Select(player => player),
                    (x, player) => new {x.r, x.p, player.Vip, player.Agent})
                .GroupBy(x => new
                    {
                        x.r.BrandId,
                        x.r.AgentId,
                        x.r.PlayerCode,
                        x.r.GameProviderId,
                        x.r.GameType,
                        x.Vip,
                        x.Agent
                    },
                    (x, aggregate) =>
                    {
                        return new PlayerGameTypeAggregateModel
                        {
                            BrandId = x.BrandId,
                            AgentId = x.AgentId,
                            PlayerCode = x.PlayerCode,
                            VipLevel = x.Vip.Level,
                            GameProviderId = x.GameProviderId,
                            GameType = x.GameType,
                            Hour = start.Hour,
                            VipSportsRebatePercentage = x.Vip.SportsRebatePercentage,
                            VipVirtualSportsRebatePercentage = x.Vip.CardGameRebatePercentage,
                            VipLiveDealerRebatePercentage = x.Vip.LiveDealerRebatePercentage,
                            VipCardGameRebatePercentage = x.Vip.CardGameRebatePercentage,
                            VipLotteryRebatePercentage = x.Vip.LotteryRebatePercentage,
                            VipSlotRebatePercentage = x.Vip.SlotRebatePercentage,
                            Turnover = aggregate.Sum(a => a.r.Turnover),
                            WinLoss = aggregate.Sum(a => a.r.WinLoss),
                            VipRebateAmount =
                                GetPlayerRebateAmountByGameType(x.Vip, x.GameType, aggregate.Select(a => a.r)),
                            AgentDividendPercentage = x.Agent?.DividendPercentage ?? 0,
                            CreatedOn = DateTimeOffset.Now,
                            ReportDate = reportDate
                        };
                    }).ToArray();

            return (playerAggregates, playerAggregateByGameType);
        }

        private static decimal GetPlayerRebateAmountByGameType(VipLevel vip, GameType type,
            IEnumerable<RoundAggregate> roundAggregates)
        {
            switch (type)
            {
                case GameType.Sportsbook:
                case GameType.ESports:
                    return vip.SportsRebatePercentage * roundAggregates.Where(a =>
                            a.GameType == GameType.Sportsbook || a.GameType == GameType.ESports)
                        .Sum(a => a.Turnover) / 100;
                case GameType.VirtualSports:
                    return vip.SportsRebatePercentage * roundAggregates.Where(a =>
                            a.GameType == GameType.VirtualSports)
                        .Sum(a => a.Turnover) / 100;
                case GameType.LiveDealer:
                    return vip.SportsRebatePercentage * roundAggregates.Where(a =>
                            a.GameType == GameType.LiveDealer)
                        .Sum(a => a.Turnover) / 100;
                case GameType.CardGame:
                    return vip.SportsRebatePercentage * roundAggregates.Where(a =>
                            a.GameType == GameType.Sportsbook || a.GameType == GameType.CardGame)
                        .Sum(a => a.Turnover) / 100;
                case GameType.Fishing:
                case GameType.Arcade:
                case GameType.Slot:
                case GameType.TableGame:
                    return vip.SportsRebatePercentage * roundAggregates.Where(a =>
                            a.GameType == GameType.Fishing || a.GameType == GameType.Arcade ||
                            a.GameType == GameType.Slot || a.GameType == GameType.TableGame)
                        .Sum(a => a.Turnover) / 100;
                case GameType.Lottery:
                    return vip.SportsRebatePercentage * roundAggregates.Where(a =>
                            a.GameType == GameType.Lottery)
                        .Sum(a => a.Turnover) / 100;
                default:
                    return 0;
            }
        }
    }
}
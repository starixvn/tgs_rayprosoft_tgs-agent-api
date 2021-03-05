using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Report.Application.Models.Market;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IRoundService
    {
        ITgsRepository Repository { get; }

        IQueryable<Round> GetRoundsAsync(Func<IQueryable<Round>, IQueryable<Round>> preQuery = null,
            bool tracking = false);

        Task<MarketRequest[]> GetRoundsAsync(DateTimeOffset start, DateTimeOffset end);
    }

    public class RoundService : BaseRepositoryOperations<ITgsRepository>, IRoundService
    {
        public readonly ITgsRepository _repository;

        public RoundService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IRoundService.Repository => _repository;

        IQueryable<Round> IRoundService.GetRoundsAsync(
            Func<IQueryable<Round>, IQueryable<Round>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task<MarketRequest[]> IRoundService.GetRoundsAsync(DateTimeOffset start, DateTimeOffset end) =>
            (from brand in _repository.Brands
             join player in _repository.Players on brand equals player
                 .Brand into bpGroup
             from p in bpGroup.DefaultIfEmpty()
             join transaction in _repository.Transactions.Where(x =>
                     x.ConfirmedOn >= start && x.ConfirmedOn < end)
                 on brand equals transaction.Brand into btGroup
             from t in btGroup.DefaultIfEmpty()
             join round in _repository.Rounds.Where(x =>
                     x.ClosedOn >= start && x.ClosedOn < end) on
                 brand.Id equals
                 round.BrandId into brGroup
             from r in brGroup.DefaultIfEmpty()
             select new {brandId = brand.Id, player = p, transaction = t, round = r}
             into x
             group x by x.brandId
             into g
             select new MarketRequest
             {
                 BrandId = g.Key,
                 Hour = start.Hour,
                 ReportDate = new(start.Year, start.Month, start.Day, 0,
                     0, 0,
                     start.Offset),
                 RegisteredCount = g.Count(x =>
                     x.player.RegisteredOn >= start &&
                     x.player.RegisteredOn < end),
                 FirstFundInCount = g.Count(x =>
                     x.player.FirstDepositOn >= start &&
                     x.player.FirstDepositOn < end),
                 DepositAmount = g.Where(x =>
                         x.transaction.Type == TransactionType.Deposit)
                     .Sum(x => x.transaction.Amount),
                 WithdrawalAmount = g
                     .Where(x => x.transaction.Type ==
                         TransactionType.Withdrawal)
                     .Sum(x => x.transaction.Amount),
                 AdjustmentAmount = g.Where(x =>
                         x.transaction.Type == TransactionType.Adjust)
                     .Sum(x => x.transaction.Amount),
                 BonusAmount = 0,
                 BetAmount = g.Sum(x => x.round.BetAmount) ?? 0,
                 ValidBetAmount = g.Sum(x => x.round.Turnover) ?? 0,
                 WinLossAmount = g.Sum(x => x.round.WinLoss) ?? 0,
                 NetProfit = g.Sum(x => x.round.WinLoss) -
                     g.Sum(x => x.round.Turnover) ?? 0,
                 CreatedOn = DateTimeOffset.Now
             }).ToArrayAsync();
    }
}
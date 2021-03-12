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

        IQueryable<Round> GetRoundQueries(Func<IQueryable<Round>, IQueryable<Round>> preQuery = null,
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

        IQueryable<Round> IRoundService.GetRoundQueries(
            Func<IQueryable<Round>, IQueryable<Round>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task<MarketRequest[]> IRoundService.GetRoundsAsync(DateTimeOffset start, DateTimeOffset end) =>
            _repository.Brands
                .Select(x =>
                    new MarketRequest
                    {
                        BrandId = x.Id,
                        Hour = start.Hour,
                        ReportDate = new(start.Year, start.Month, start.Day, 0,
                            0, 0,
                            start.Offset),
                        RegisteredCount =
                            _repository.Players.Count(p =>
                                p.BrandId == x.Id && p.RegisteredOn >= start && p.RegisteredOn < end),
                        FirstFundInCount =
                            _repository.Players.Count(p =>
                                p.BrandId == x.Id && p.FirstDepositOn >= start && p.FirstDepositOn < end),
                        DepositAmount =
                            _repository.Transactions.Where(t =>
                                t.BrandId == x.Id && t.Type == TransactionType.Deposit && t.ConfirmedOn >= start &&
                                t.ConfirmedOn < end).Sum(t => t.Amount),
                        WithdrawalAmount =
                            _repository.Transactions.Where(t =>
                                t.BrandId == x.Id && t.Type == TransactionType.Withdrawal && t.ConfirmedOn >= start &&
                                t.ConfirmedOn < end).Sum(t => t.Amount),
                        AdjustmentAmount = _repository.Transactions.Where(t =>
                            t.BrandId == x.Id && t.Type == TransactionType.Adjust && t.ConfirmedOn >= start &&
                            t.ConfirmedOn < end).Sum(t => t.Amount),
                        BetAmount =
                            _repository.Rounds.Where(r => r.BrandId == x.Id && r.ClosedOn >= start && r.ClosedOn < end)
                                .Sum(r => r.BetAmount) ?? 0,
                        ValidBetAmount =
                            _repository.Rounds.Where(r => r.BrandId == x.Id && r.ClosedOn >= start && r.ClosedOn < end)
                                .Sum(r => r.Turnover) ?? 0,
                        WinLossAmount = _repository.Rounds
                            .Where(r => r.BrandId == x.Id && r.ClosedOn >= start && r.ClosedOn < end)
                            .Sum(r => r.WinLoss) ?? 0,
                        CreatedOn = DateTimeOffset.Now
                    }).ToArrayAsync();
    }
}
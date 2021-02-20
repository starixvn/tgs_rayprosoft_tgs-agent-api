using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Market;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    public interface IMarketReportService
    {
        Task<TgsResponse> ExecutionAsync(string dateTime = null, int hour = 0, TimeSpan timeSpan = default);
    }

    public class MarketReportService : IMarketReportService
    {
        private readonly ITgsLog _log;
        private readonly ITgsRepository _repository;
        private readonly IMarketService _service;
        private readonly ITransactionManager _transactionManager;

        public MarketReportService(
            ITgsRepository repository,
            ITransactionManager transactionManager,
            IMarketService service,
            ITgsLog log)
        {
            _service = service;
            _transactionManager = transactionManager;
            _repository = repository;
            _log = log.SetLogger(nameof(MarketReportWorker));
        }

        async Task<TgsResponse> IMarketReportService.ExecutionAsync(string dateTime, int hour,
            TimeSpan timeSpan)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            var canDelete = false;
            var src = DateTimeOffset.Now.AddHours(-1);
            if (dateTime.IsNotNullOrEmpty() &&
                DateTimeOffset.TryParse($"{dateTime} {hour}:00 +{timeSpan}", out var result))
            {
                src = result;
                canDelete = true;
            }

            var start = new DateTimeOffset(src.Year, src.Month, src.Day, src.Hour,
                0, 0, src.Offset);
            var end = new DateTimeOffset(src.Year, src.Month, src.Day, src.Hour,
                59, 59, src.Offset);

            try
            {
                var response = await _transactionManager.ExecuteTransactionAsync(() => _repository,
                    IsolationLevel.ReadCommitted, id,
                    async _ =>
                    {
                        if (canDelete)
                        {
                            var markets = await _service.GetMarketsAsync()
                                .Where(x => x.ReportDateString == dateTime && x.Hour == hour)
                                .ToArrayAsync();
                            await _service.DeleteMarketsAsync(markets);
                        }

                        if (await _service.ExistsAsync(x =>
                            x.ReportDateString == src.ToString("yyyy/MM/dd") && x.Hour == src.Hour))
                        {
                            return new TgsResponse();
                        }

                        var brandIds = await _repository.Brands.Select(x => x.Id).ToArrayAsync();
                        var request = brandIds.GroupJoin(_repository.Players, guid => guid, p => p.BrandId,
                                (guid, p) =>
                                    new
                                    {
                                        brandId = guid,
                                        players = p.Select(x => new {x.RegisteredOn, x.FirstDepositOn})
                                    })
                            .GroupJoin(_repository.Transactions.Where(x =>
                                    x.ConfirmedOn >= start && x.ConfirmedOn <= end), c => c.brandId,
                                t => t.BrandId, (c, t) => new
                                {
                                    c.brandId,
                                    c.players,
                                    transaction = t.Select(x => new {x.Type, x.Amount})
                                })
                            .GroupJoin(
                                _repository.Rounds.Where(x =>
                                    x.ClosedOn >= start && x.ClosedOn <= end), c => c.brandId,
                                r => r.BrandId, (c, r) => new CreateMarketRequest
                                {
                                    BrandId = c.brandId,
                                    Hour = src.Hour,
                                    ReportDateString = src.ToString("yyyy/MM/dd"),
                                    RegisteredCount = c.players.Count(x =>
                                        x.RegisteredOn >= start && x.RegisteredOn <= end),
                                    FirstFundInCount = c.players.Count(x =>
                                        x.FirstDepositOn >= start && x.FirstDepositOn <= end),
                                    DepositAmount = c.transaction.Where(x => x.Type == TransactionType.Deposit)
                                        .Sum(x => x.Amount),
                                    WithdrawalAmount = c.transaction.Where(x => x.Type == TransactionType.Withdrawal)
                                        .Sum(x => x.Amount),
                                    AdjustmentAmount = c.transaction.Where(x => x.Type == TransactionType.Adjust)
                                        .Sum(x => x.Amount),
                                    BonusAmount = 0,
                                    BetAmount = r.Sum(x => x.BetAmount) ?? 0,
                                    ValidBetAmount = r.Sum(x => x.Turnover) ?? 0,
                                    WinLossAmount = r.Sum(x => x.WinLoss) ?? 0,
                                    NetProfit = r.Sum(x => x.WinLoss) - r.Sum(x => x.Turnover) ?? 0,
                                    CreatedOn = DateTimeOffset.Now
                                }).ToArray();

                        await _service.CreateMarketsAsync(request);

                        return new TgsResponse();
                    });

                return response;
            }
            catch (Exception ex)
            {
                _log.LogError("Fail to insert market report", ex);

                return new TgsResponse
                {
                    Error = (int) TgsApiError.SystemError,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
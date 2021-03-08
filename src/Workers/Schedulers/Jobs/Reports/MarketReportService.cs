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
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    public interface IMarketReportService
    {
        Task<TgsResponse> ExecutionAsync(string date = null, int hour = 0);
    }

    public class MarketReportService : IMarketReportService
    {
        private readonly ITgsLog _log;
        private readonly ITgsRepository _repository;
        private readonly IRoundService _roundService;
        private readonly IMarketService _service;
        private readonly ITransactionManager _transactionManager;

        public MarketReportService(
            ITgsRepository repository,
            ITransactionManager transactionManager,
            IMarketService service,
            IRoundService roundService,
            ITgsLog log)
        {
            _service = service;
            _roundService = roundService;
            _transactionManager = transactionManager;
            _repository = repository;
            _log = log.SetLogger(nameof(MarketReportWorker));
        }

        async Task<TgsResponse> IMarketReportService.ExecutionAsync(string date, int hour)
        {
            try
            {
                var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                var canDelete = false;
                var previousHour = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddHours(-1);
                var theDay = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, 0, 0, 0,
                    previousHour.Offset);

                var marketsOfDay = await _service.GetMarketsAsync()
                    .Where(x => x.ReportDate == theDay)
                    .Select(x => x.Hour).Distinct().ToArrayAsync();

                if (date.IsNotNullOrEmpty() &&
                    DateTimeOffset.TryParse($"{date} {hour}:00 +08:00", out var result))
                {
                    previousHour = result;
                    canDelete = true;
                    marketsOfDay = marketsOfDay.Where(x => x != hour).ToArray();
                }

                var hourArray = Enumerable.Range(0, previousHour.Hour + 1);

                var doHours = hourArray.Except(marketsOfDay).ToArray();

                if (doHours.Length == 0)
                {
                    return new() {ErrorMessage = "Not any hours to generate market report."};
                }

                foreach (var doHour in doHours)
                {
                    var start = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, doHour,
                        0, 0, previousHour.Offset);
                    var end = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, doHour,
                        0, 0, previousHour.Offset).AddHours(1);

                    await _transactionManager.ExecuteTransactionAsync(() => _repository,
                        IsolationLevel.ReadCommitted, id,
                        async _ =>
                        {
                            if (canDelete)
                            {
                                var markets = await _service.GetMarketsAsync()
                                    .Where(x => x.ReportDate == DateTimeOffset.Parse(date) && x.Hour == hour)
                                    .ToArrayAsync();
                                await _service.DeleteMarketsAsync(markets);
                                canDelete = false;
                            }

                            if (await _service.ExistsAsync(x =>
                                x.ReportDate == previousHour && x.Hour == start.Hour))
                            {
                                return new TgsResponse
                                {
                                    ErrorMessage = $"{theDay.Date} {start.Hour} is already existed."
                                };
                            }

                            var request = await _roundService.GetRoundsAsync(start, end);

                            await _service.CreateMarketsAsync(request);

                            return new();
                        });
                }

                return new();
            }
            catch (Exception ex)
            {
                _log.LogError("Fail to insert market report", ex);

                return new() {Error = (int)TgsApiError.SystemError, ErrorMessage = ex.Message};
            }
        }
    }
}
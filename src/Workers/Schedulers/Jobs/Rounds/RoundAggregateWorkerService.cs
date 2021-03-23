using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Report;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs.Rounds
{
    public interface IRoundAggregateWorkerService
    {
        Task<TgsResponse> ExecutionAsync(string date = null, int hour = 0);
    }

    public class RoundAggregateWorkerService : IRoundAggregateWorkerService
    {
        private readonly ITgsLog _log;
        private readonly IReportRepository _repository;
        private readonly ITgsRoundService _roundService;
        private readonly IRoundAggregateService _service;
        private readonly ITransactionManager _transactionManager;

        public RoundAggregateWorkerService(
            IRoundAggregateService service,
            ITgsRoundService roundService,
            ITransactionManager transactionManager,
            IReportRepository repository,
            ITgsLog log)
        {
            _service = service;
            _roundService = roundService;
            _transactionManager = transactionManager;
            _repository = repository;
            _log = log.SetLogger(nameof(RoundAggregateWorker));
        }

        async Task<TgsResponse> IRoundAggregateWorkerService.ExecutionAsync(string date, int hour)
        {
            try
            {
                var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                var canDelete = false;
                var previousHour = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddHours(-1);
                var theDay = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, 0, 0, 0,
                    previousHour.Offset);

                if (date.IsNotNullOrEmpty() &&
                    DateTimeOffset.TryParse($"{date} {hour}:00 +08:00", out var result))
                {
                    theDay = new(result.Year, result.Month, result.Day, 0, 0, 0,
                        result.Offset);
                    previousHour = result;
                    canDelete = true;
                }

                var marketsOfDay = await _service.GetRoundAggregateQueries()
                    .Where(x => x.ReportDate == theDay)
                    .Where(canDelete, x => x.Hour != hour)
                    .Select(x => x.Hour).Distinct().ToArrayAsync();

                var hourArray = Enumerable.Range(0, previousHour.Hour + 1);

                var doHours = hourArray.Except(marketsOfDay).ToArray();

                if (doHours.Length == 0)
                {
                    return new() {ErrorMessage = "Not any hours to generate round aggregate."};
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
                                var roundAggregates = await _service.GetRoundAggregateQueries()
                                    .Where(x => x.ReportDate == DateTimeOffset.Parse(date) && x.Hour == hour)
                                    .ToArrayAsync();
                                await _service.DeleteRoundAggregatesAsync(roundAggregates);
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

                            var models = await _roundService.GetRoundsAsync(start, end);

                            if (models.Length > 0)
                            {
                                await _service.CreateRoundAggregateAsync(models);
                            }

                            return new();
                        });
                }

                return new();
            }
            catch (Exception ex)
            {
                _log.LogError("Fail to insert round aggregate", ex);

                return new() {Error = (int)TgsApiError.SystemError, ErrorMessage = ex.Message};
            }
        }
    }
}
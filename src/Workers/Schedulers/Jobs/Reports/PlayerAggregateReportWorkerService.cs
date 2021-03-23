using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    public interface IPlayerAggregateReportWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
        Task<TgsResponse> ManualExecutionAsync(string date = null, int hour = 0);
    }

    public class PlayerAggregateReportWorkerService : IPlayerAggregateReportWorkerService
    {
        private readonly ITgsLog _log;
        private readonly IPlayerGameTypeAggregateService _playerGameTypeService;
        private readonly IRoundAggregateService _roundAggregateService;
        private readonly IPlayerAggregateService _service;

        private readonly ITransactionManager _transactionManager;

        public PlayerAggregateReportWorkerService(
            IPlayerAggregateService service,
            IPlayerGameTypeAggregateService playerGameTypeService,
            IRoundAggregateService roundAggregateService,
            ITransactionManager transactionManager,
            ITgsLog log)
        {
            _service = service;
            _playerGameTypeService = playerGameTypeService;
            _roundAggregateService = roundAggregateService;
            _transactionManager = transactionManager;
            _log = log.SetLogger(nameof(PlayerAggregateReportWorker));
        }

        async Task<TgsResponse> IPlayerAggregateReportWorkerService.ExecutionAsync()
        {
            try
            {
                var previousHour = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddHours(-1);
                var reportDate = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, 0, 0, 0,
                    previousHour.Offset);

                var marketsOfDay = await _service.GetPlayerAggregateQueries()
                    .Where(x => x.ReportDate == reportDate)
                    .Select(x => x.Hour).Distinct().ToArrayAsync();

                var hourArray = Enumerable.Range(0, previousHour.Hour + 1);

                var doHours = hourArray.Except(marketsOfDay).ToArray();

                if (doHours.Length == 0)
                {
                    return new() {ErrorMessage = "Not any hours to generate player aggregate."};
                }

                foreach (var doHour in doHours)
                {
                    var start = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, doHour,
                        0, 0, previousHour.Offset);
                    var end = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, doHour,
                        0, 0, previousHour.Offset).AddHours(1);

                    if (await _service.ExistsAsync(x =>
                        x.ReportDate == reportDate && x.Hour == start.Hour))
                    {
                        return new() {ErrorMessage = $"{reportDate.Date} {start.Hour} is already existed."};
                    }

                    var (aggregates, aggregatesByGameType) =
                        await _roundAggregateService.GetPlayerAggregatesAsync(start, end);

                    if (aggregates.Length > 0)
                    {
                        await _service.CreatePlayerAggregatesAsync(aggregates);
                    }

                    if (aggregatesByGameType.Length > 0)
                    {
                        await _service.CreatePlayerAggregatesByGameTypeAsync(aggregatesByGameType);
                    }
                }

                return new();
            }
            catch (Exception ex)
            {
                _log.LogError("Fail to insert market report", ex);

                return new() {Error = (int)TgsApiError.SystemError, ErrorMessage = ex.Message};
            }
        }

        async Task<TgsResponse> IPlayerAggregateReportWorkerService.ManualExecutionAsync(string date, int hour)
        {
            try
            {
                var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                DateTimeOffset reportDate;

                if (date.IsNotNullOrEmpty() &&
                    DateTimeOffset.TryParse($"{date} {hour}:00 +08:00", out var result))
                {
                    reportDate = new(result.Year, result.Month, result.Day, 0, 0, 0,
                        result.Offset);
                }
                else
                {
                    return new() {ErrorMessage = "Date is invalid."};
                }

                var marketsOfDay = await _service.GetPlayerAggregateQueries()
                    .Where(x => x.ReportDate == reportDate)
                    .Where(x => x.Hour != hour)
                    .Select(x => x.Hour).Distinct().ToArrayAsync();

                var hourArray = Enumerable.Range(0, result.Hour + 1);

                var doHours = hourArray.Except(marketsOfDay).ToArray();

                if (doHours.Length == 0)
                {
                    return new() {ErrorMessage = "Not any hours to generate player aggregate."};
                }

                foreach (var doHour in doHours)
                {
                    var start = new DateTimeOffset(result.Year, result.Month, result.Day, doHour,
                        0, 0, result.Offset);
                    var end = new DateTimeOffset(result.Year, result.Month, result.Day, doHour,
                        0, 0, result.Offset).AddHours(1);

                    await _transactionManager.ExecuteTransactionAsync(() => _service.Repository,
                        IsolationLevel.ReadCommitted, id,
                        async _ =>
                        {
                            var playerAggregates = await _service.GetPlayerAggregateQueries()
                                .Where(x => x.ReportDate == DateTimeOffset.Parse(date) && x.Hour == hour)
                                .ToArrayAsync();

                            await _service.DeletePlayerAggregatesAsync(playerAggregates);

                            var playerGameTypeAggregates = await _playerGameTypeService
                                .GetPlayerGameTypeAggregateQueries()
                                .Where(x => x.ReportDate == DateTimeOffset.Parse(date) && x.Hour == hour)
                                .ToArrayAsync();

                            await _playerGameTypeService.DeletePlayerGameTypeAggregatesAsync(playerGameTypeAggregates);

                            if (await _service.ExistsAsync(x =>
                                x.ReportDate == reportDate && x.Hour == start.Hour))
                            {
                                return new TgsResponse
                                {
                                    ErrorMessage = $"{reportDate.Date} {start.Hour} is already existed."
                                };
                            }

                            var (aggregates, aggregatesByGameType) =
                                await _roundAggregateService.GetPlayerAggregatesAsync(start, end);

                            if (aggregates.Length > 0)
                            {
                                await _service.CreatePlayerAggregatesAsync(aggregates);
                            }

                            if (aggregatesByGameType.Length > 0)
                            {
                                await _playerGameTypeService.CreatePlayerGameTypeAggregatesAsync(aggregatesByGameType);
                            }

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
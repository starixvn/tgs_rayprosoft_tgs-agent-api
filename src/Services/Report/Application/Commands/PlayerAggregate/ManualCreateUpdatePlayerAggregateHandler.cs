using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Aggregate;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.PlayerAggregate
{
    public class
        ManualCreateUpdatePlayerAggregateHandler : Handler<ManualCreateUpdatePlayerAggregateModel,
            MessageResponse>
    {
        private readonly IPlayerAggregateService _playerAggregateService;
        private readonly IPlayerGameTypeAggregateService _playerGameTypeAggregateService;
        private readonly IRoundAggregateService _roundAggregateService;
        private readonly ITransactionManager _transactionManager;

        public ManualCreateUpdatePlayerAggregateHandler(IPlayerAggregateService playerAggregateService,
            IPlayerGameTypeAggregateService playerGameTypeAggregateService,
            IRoundAggregateService roundAggregateService,
            ITransactionManager transactionManager,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _playerAggregateService = playerAggregateService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _roundAggregateService = roundAggregateService;
            _transactionManager = transactionManager;
        }

        public override async Task<MessageResponse> Handle(ManualCreateUpdatePlayerAggregateModel request,
            CancellationToken cancellationToken)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            DateTimeOffset reportDate;

            if (request.Date.IsNotNullOrEmpty() &&
                DateTimeOffset.TryParse($"{request.Date} {request.Hour}:00 +08:00", out var result))
            {
                reportDate = new(result.Year, result.Month, result.Day, 0, 0, 0,
                    result.Offset);
            }
            else
            {
                return CreateInvalidResponse<MessageResponse>("Date is invalid.");
            }

            var marketsOfDay = await _playerAggregateService.GetPlayerAggregateQueries()
                .Where(x => x.ReportDate == reportDate)
                .Where(x => x.Hour != request.Hour)
                .Select(x => x.Hour).Distinct().ToArrayAsync(cancellationToken);

            var hourArray = Enumerable.Range(0, result.Hour + 1);

            var doHours = hourArray.Except(marketsOfDay).ToArray();

            if (doHours.Length == 0)
            {
                return CreateInvalidResponse<MessageResponse>("Not any hours to generate player aggregate.");
            }

            foreach (var doHour in doHours)
            {
                var start = new DateTimeOffset(result.Year, result.Month, result.Day, doHour,
                    0, 0, result.Offset);
                var end = new DateTimeOffset(result.Year, result.Month, result.Day, doHour,
                    0, 0, result.Offset).AddHours(1);

                var response = await _transactionManager.ExecuteTransactionAsync(
                    () => _playerAggregateService.Repository,
                    IsolationLevel.ReadCommitted, id,
                    async _ =>
                    {
                        var playerAggregates = await _playerAggregateService.GetPlayerAggregateQueries()
                            .Where(x => x.ReportDate == DateTimeOffset.Parse(request.Date) && x.Hour == request.Hour)
                            .ToArrayAsync(cancellationToken);

                        await _playerAggregateService.DeletePlayerAggregatesAsync(playerAggregates);

                        var playerGameTypeAggregates = await _playerGameTypeAggregateService
                            .GetPlayerGameTypeAggregateQueries()
                            .Where(x => x.ReportDate == DateTimeOffset.Parse(request.Date) && x.Hour == request.Hour)
                            .ToArrayAsync(cancellationToken);

                        await _playerGameTypeAggregateService.DeletePlayerGameTypeAggregatesAsync(
                            playerGameTypeAggregates);

                        if (await _playerAggregateService.ExistsAsync(x =>
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
                            await _playerAggregateService.CreatePlayerAggregatesAsync(aggregates);
                        }

                        if (aggregatesByGameType.Length > 0)
                        {
                            await _playerGameTypeAggregateService.CreatePlayerGameTypeAggregatesAsync(
                                aggregatesByGameType);
                        }

                        return new();
                    });

                if (response.ErrorMessage.IsNotNullOrEmpty())
                {
                    return CreateInvalidResponse<MessageResponse>(response.ErrorMessage);
                }
            }

            return new() {Valid = true};
        }
    }
}
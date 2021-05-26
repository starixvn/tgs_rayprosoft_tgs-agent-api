using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
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
        private readonly IPlayerGameAggregateService _playerGameTypeAggregateService;
        private readonly IRoundAggregateService _roundAggregateService;

        public ManualCreateUpdatePlayerAggregateHandler(
            IPlayerAggregateService playerAggregateService,
            IPlayerGameAggregateService playerGameTypeAggregateService,
            IRoundAggregateService roundAggregateService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _playerAggregateService = playerAggregateService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _roundAggregateService = roundAggregateService;
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

                if (await _playerAggregateService.ExistsAsync(x =>
                    x.ReportDate == reportDate && x.Hour == start.Hour))
                {
                    return CreateInvalidResponse<MessageResponse>(
                        $"{reportDate.Date} {start.Hour} is already existed.");
                }

                var (aggregates, aggregatesByGameType) =
                    await _roundAggregateService.GetPlayerAggregatesAsync(start, end);

                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                if (aggregates.Length > 0)
                {
                    await _playerAggregateService.CreatePlayerAggregatesAsync(aggregates);
                }

                if (aggregatesByGameType.Length > 0)
                {
                    await _playerGameTypeAggregateService.CreatePlayerGameAggregatesAsync(
                        aggregatesByGameType);
                }

                scope.Complete();
            }

            return new() {Valid = true};
        }
    }
}
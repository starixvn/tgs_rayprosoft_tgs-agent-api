using System;
using System.Data;
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

namespace TGS.Services.Report.Application.Commands.RoundAggregate
{
    public class
        ManualCreateUpdateRoundAggregateHandler : Handler<ManualCreateUpdateRoundAggregateModel,
            MessageResponse>
    {
        private readonly IRoundAggregateService _roundAggregateService;
        private readonly ITgsRoundService _roundService;

        public ManualCreateUpdateRoundAggregateHandler(ITgsRoundService roundService,
            IRoundAggregateService roundAggregateService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _roundService = roundService;
            _roundAggregateService = roundAggregateService;
        }

        public override async Task<MessageResponse> Handle(ManualCreateUpdateRoundAggregateModel request,
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

            var marketsOfDay = await _roundAggregateService.GetRoundAggregateQueries()
                .Where(x => x.ReportDate == reportDate)
                .Where(x => x.Hour != request.Hour)
                .Select(x => x.Hour).Distinct().ToArrayAsync(cancellationToken);

            var hourArray = Enumerable.Range(0, result.Hour + 1);

            var doHours = hourArray.Except(marketsOfDay).ToArray();

            if (doHours.Length == 0)
            {
                return CreateInvalidResponse<MessageResponse>("Not any hours to generate round aggregate.");
            }

            foreach (var doHour in doHours)
            {
                var start = new DateTimeOffset(result.Year, result.Month, result.Day, doHour,
                    0, 0, result.Offset);
                var end = new DateTimeOffset(result.Year, result.Month, result.Day, doHour,
                    0, 0, result.Offset).AddHours(1);

                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var playerAggregates = await _roundAggregateService.GetRoundAggregateQueries()
                    .Where(x => x.ReportDate == DateTimeOffset.Parse(request.Date) && x.Hour == request.Hour)
                    .ToArrayAsync(cancellationToken);

                await _roundAggregateService.DeleteRoundAggregatesAsync(playerAggregates);

                var playerGameTypeAggregates = await _roundAggregateService
                    .GetRoundAggregateQueries()
                    .Where(x => x.ReportDate == DateTimeOffset.Parse(request.Date) && x.Hour == request.Hour)
                    .ToArrayAsync(cancellationToken);

                await _roundAggregateService.DeleteRoundAggregatesAsync(playerGameTypeAggregates);

                if (await _roundAggregateService.ExistsAsync(x =>
                    x.ReportDate == reportDate && x.Hour == start.Hour))
                {
                    scope.Complete();
                    return CreateInvalidResponse<MessageResponse>($"{reportDate.Date} {start.Hour} is already existed.");
                }

                var models = await _roundService.GetRoundsAsync(start, end);

                if (models.Length > 0)
                {
                    await _roundAggregateService.CreateRoundAggregateAsync(models);
                }

                scope.Complete();
            }

            return new() { Valid = true };
        }
    }
}

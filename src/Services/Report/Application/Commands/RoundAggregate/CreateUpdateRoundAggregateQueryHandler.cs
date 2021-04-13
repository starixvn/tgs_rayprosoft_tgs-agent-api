using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Aggregate;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.RoundAggregate
{
    public class
        CreateUpdateRoundAggregateQueryHandler : Handler<CreateUpdateRoundAggregateModel,
            MessageResponse>
    {
        private readonly IRoundAggregateService _roundAggregateService;
        private readonly ITgsRoundService _roundService;

        public CreateUpdateRoundAggregateQueryHandler(ITgsRoundService roundService,
            IRoundAggregateService roundAggregateService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _roundService = roundService;
            _roundAggregateService = roundAggregateService;
        }

        public override async Task<MessageResponse> Handle(CreateUpdateRoundAggregateModel request,
            CancellationToken cancellationToken)
        {
            var previousHour = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddHours(-1);
            var theDay = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, 0, 0, 0,
                previousHour.Offset);

            var marketsOfDay = await _roundAggregateService.GetRoundAggregateQueries()
                .Where(x => x.ReportDate == theDay)
                .Select(x => x.Hour).Distinct().ToArrayAsync(cancellationToken);

            var hourArray = Enumerable.Range(0, previousHour.Hour + 1);

            var doHours = hourArray.Except(marketsOfDay).ToArray();

            foreach (var doHour in doHours)
            {
                var start = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, doHour,
                    0, 0, previousHour.Offset);
                var end = new DateTimeOffset(previousHour.Year, previousHour.Month, previousHour.Day, doHour,
                    0, 0, previousHour.Offset).AddHours(1);

                if (await _roundAggregateService.ExistsAsync(x =>
                    x.ReportDate == previousHour && x.Hour == start.Hour))
                {
                    return CreateInvalidResponse<MessageResponse>($"{theDay.Date} {start.Hour} is already existed.");
                }

                var models = await _roundService.GetRoundsAsync(start, end);

                if (models.Length > 0)
                {
                    await _roundAggregateService.CreateRoundAggregateAsync(models);
                }
            }

            return new() {Valid = true};
        }
    }
}
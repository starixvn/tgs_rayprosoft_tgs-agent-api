using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Models.Messages;
using Tgs.MessageQueue;
using TGS.Services.Report.Application.Models.Aggregate;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.PlayerAggregate
{
    public class
        CreateUpdatePlayerAggregateHandler : Handler<CreateUpdatePlayerAggregateModel,
            MessageResponse>
    {
        private readonly IMessageSender _messageSender;
        private readonly IPlayerAggregateService _playerAggregateService;
        private readonly IPlayerGameAggregateService _playerGameTypeAggregateService;
        private readonly IRoundAggregateService _roundAggregateService;

        public CreateUpdatePlayerAggregateHandler(IPlayerAggregateService playerAggregateService,
            IPlayerGameAggregateService playerGameTypeAggregateService,
            IRoundAggregateService roundAggregateService,
            IMessageSender messageSender,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _playerAggregateService = playerAggregateService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _roundAggregateService = roundAggregateService;
            _messageSender = messageSender;
        }

        public override async Task<MessageResponse> Handle(CreateUpdatePlayerAggregateModel request,
            CancellationToken cancellationToken)
        {
            var previousHour = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddHours(-1);
            var reportDate = new DateTimeOffset(previousHour.Year,
                previousHour.Month,
                previousHour.Day,
                0,
                0,
                0,
                previousHour.Offset);

            var marketsOfDay = await _playerAggregateService.GetPlayerAggregateQueries()
                .Where(x => x.ReportDate == reportDate)
                .Select(x => x.Hour).Distinct().ToArrayAsync(cancellationToken);

            var hourArray = Enumerable.Range(0, previousHour.Hour + 1);

            var doHours = hourArray.Except(marketsOfDay).ToArray();

            if (doHours.Length == 0)
            {
                return CreateInvalidResponse<MessageResponse>("Not any hours to generate player aggregate.");
            }

            foreach (var doHour in doHours)
            {
                var start = new DateTimeOffset(previousHour.Year,
                    previousHour.Month,
                    previousHour.Day,
                    doHour,
                    0,
                    0,
                    previousHour.Offset);
                var end = new DateTimeOffset(previousHour.Year,
                    previousHour.Month,
                    previousHour.Day,
                    doHour,
                    0,
                    0,
                    previousHour.Offset).AddHours(1);

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
                    await _playerGameTypeAggregateService.CreatePlayerGameAggregatesAsync(aggregatesByGameType);
                }

                scope.Complete();

                var sendForVipLevelUpCalculation = new
                {
                    ReportOn = reportDate,
                    PlayerCodes = aggregates.Select(x => x.PlayerCode).ToArray()
                };

                await _messageSender.PublishAsync("tgs.viplevel.aggregate.server",
                        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sendForVipLevelUpCalculation)))
                    .ConfigureAwait(false);
            }

            return new() { Valid = true };
        }
    }
}
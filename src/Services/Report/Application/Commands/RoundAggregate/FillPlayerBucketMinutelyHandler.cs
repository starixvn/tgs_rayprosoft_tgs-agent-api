using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.Report.Application.Models.Aggregate;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.RoundAggregate
{
    public class
        FillPlayerBucketMinutelyHandler : Handler<FillPlayerBucketMinutelyRequest,
            MessageResponse>
    {
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly ITgsRoundService _roundService;

        public FillPlayerBucketMinutelyHandler(ITgsRoundService roundService,
            IPlayerBucketRepository playerBucketRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _roundService = roundService;
            _playerBucketRepository = playerBucketRepository;
        }

        public override async Task<MessageResponse> Handle(FillPlayerBucketMinutelyRequest request,
            CancellationToken cancellationToken)
        {
            var previousMinute = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddMinutes(-1);

            var start = new DateTimeOffset(previousMinute.Year, previousMinute.Month, previousMinute.Day,
                previousMinute.Hour,
                previousMinute.Minute, 0, previousMinute.Offset);

            var end = new DateTimeOffset(previousMinute.Year, previousMinute.Month, previousMinute.Day,
                previousMinute.Hour,
                previousMinute.Minute, 0, previousMinute.Offset).AddMinutes(1);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var models = await _roundService.GetMinuteRoundAggregateAsync(start, end);

            if (models.Length > 0)
            {
                var playerTurnovers = models.GroupBy(x => x.PlayerCode,
                        (playerCode, list) => new {playerCode, list})
                    .Select(x => new PlayerTurnoverModel
                    {
                        PlayerCode = x.playerCode, Turnover = x.list.Sum(l => l.Turnover)
                    }).ToArray();

                await _playerBucketRepository.FillInBucketAsync(
                    new() {From = start, To = end, PlayerTurnovers = playerTurnovers});
            }

            scope.Complete();

            return new() {Valid = true};
        }
    }
}
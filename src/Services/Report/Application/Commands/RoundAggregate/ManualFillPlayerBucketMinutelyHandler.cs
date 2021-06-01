using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.Report.Application.Models.Aggregate;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.RoundAggregate
{
    public class
        ManualFillPlayerBucketMinutelyHandler : Handler<ManualFillPlayerBucketMinutelyRequest,
            MessageResponse>
    {
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly ITgsRoundService _roundService;

        public ManualFillPlayerBucketMinutelyHandler(ITgsRoundService roundService,
            IPlayerBucketRepository playerBucketRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _roundService = roundService;
            _playerBucketRepository = playerBucketRepository;
        }

        public override async Task<MessageResponse> Handle(ManualFillPlayerBucketMinutelyRequest request,
            CancellationToken cancellationToken)
        {
            if (request.Date.IsNotNullOrEmpty() &&
                DateTimeOffset.TryParse($"{request.Date} {request.Hour}:{request.FromMinute} +08:00", out var result) &&
                DateTimeOffset.TryParse($"{request.Date} {request.Hour}:{request.ToMinute} +08:00", out _))
            {
                var start = new DateTimeOffset(result.Year, result.Month, result.Day,
                    result.Hour,
                    request.FromMinute, 0, result.Offset);

                var end = new DateTimeOffset(result.Year, result.Month, result.Day,
                    result.Hour,
                    request.ToMinute, 0, result.Offset).AddMinutes(1);

                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var models = await _roundService.GetMinuteRoundAggregateAsync(start, end);

                if (models.Length > 0 && request.FillPlayerBucket)
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

            return CreateInvalidResponse<MessageResponse>("Date is invalid.");
        }
    }
}
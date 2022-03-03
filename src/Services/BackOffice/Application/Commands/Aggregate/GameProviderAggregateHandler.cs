using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Messages;
using Tgs.MessageQueue;
using Tgs.Report.Aggregate.Client;
using TGS.Services.BackOffice.Application.Models.Aggregate;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.Aggregate
{
    public class GameProviderAggregateHandler : Handler<GenerateGameProviderAggregateRequest, MessageResponse>
    {
        private readonly IReportAggregateClient _client;
        private readonly IGameProviderAggregateAuditLogService _gameProviderAuditLogService;
        private readonly IGameProviderService _gameProviderService;
        private readonly IMessageSender _messageSender;
        private readonly IRedisCacheManager _redisCacheManager;

        public GameProviderAggregateHandler(
            IMessageSender messageSender,
            IGameProviderService gameProviderService,
            IGameProviderAggregateAuditLogService gameProviderAuditLogService,
            IRedisCacheManager redisCacheManager,
            IReportAggregateClient client,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _gameProviderService = gameProviderService;
            _messageSender = messageSender;
            _gameProviderAuditLogService = gameProviderAuditLogService;
            _redisCacheManager = redisCacheManager;
            _client = client;
        }

        public override async Task<MessageResponse> Handle(
            GenerateGameProviderAggregateRequest request,
            CancellationToken cancellationToken)
        {
            if (request.StartDate.ToOffset(new(8, 0, 0)).Day != 1)
            {
                return CreateInvalidResponse<MessageResponse>("The date must be number 1 of each month.");
            }

            var aggregate = await _redisCacheManager.GetSetAllMembersAsync("aggregate", "GP")
                .ConfigureAwait(false);

            if (aggregate.Length > 0)
            {
                return CreateInvalidResponse<MessageResponse>("waiting for report generation.");
            }

            var gameProviderIds = request.GameProviderId is not null
                ? new[]
                {
                    request.GameProviderId.Value
                }
                : await _gameProviderService.GetGameProviderQueries().Select(x => x.Id).ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

            var auditLogId =
                await _gameProviderAuditLogService.CreateGameProviderAggregateAuditLogAsync(
                        new CreateGameProviderAggregateAuditLogRequest(Guid.Parse(UserId), request.GameProviderId))
                    .ConfigureAwait(false);

            await _client.RemoveGameProviderAggregateAsync(new(request.GameProviderId, request.StartDate))
                .ConfigureAwait(false);

            await _redisCacheManager.SetAddAsync("aggregate",
                "GP",
                gameProviderIds.Select(x => (RedisValue)x.ToString()).ToArray(),
                TimeSpan.FromMinutes(10)).ConfigureAwait(false);

            foreach (var gameProviderId in gameProviderIds)
            {
                var data = new
                {
                    request.StartDate,
                    GameProviderId = gameProviderId,
                    AuditLogId = auditLogId,
                    RedisKey = "aggregate:GP"
                };

                await _messageSender.PublishAsync("tgs.report.aggregate.game.provider",
                        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)),
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            return CreateValidResponse<MessageResponse>();
        }
    }
}
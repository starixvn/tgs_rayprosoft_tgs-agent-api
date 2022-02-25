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
using TGS.Services.BackOffice.Application.Models.Aggregate;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.Aggregate
{
    public class GameProviderAggregateHandler : Handler<GenerateGameProviderAggregateRequest, MessageResponse>
    {
        private readonly IGameProviderAuditLogService _gameProviderAuditLogService;
        private readonly IGameProviderService _gameProviderService;
        private readonly IMessageSender _messageSender;
        private readonly IRedisCacheManager _redisCacheManager;

        public GameProviderAggregateHandler(
            IMessageSender messageSender,
            IGameProviderService gameProviderService,
            IGameProviderAuditLogService gameProviderAuditLogService,
            IRedisCacheManager redisCacheManager,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _gameProviderService = gameProviderService;
            _messageSender = messageSender;
            _gameProviderAuditLogService = gameProviderAuditLogService;
            _redisCacheManager = redisCacheManager;
        }

        public override async Task<MessageResponse> Handle(
            GenerateGameProviderAggregateRequest request,
            CancellationToken cancellationToken)
        {
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
                : await _gameProviderService.GetGameProviderQueries().Select(x => x.Id).ToArrayAsync(cancellationToken);

            var auditLogId =
                await _gameProviderAuditLogService.CreateGameProviderAggregateAuditLogAsync(
                    new CreateGameProviderAggregateAuditLogRequest(Guid.Parse(UserId), request.GameProviderId));

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
                    Key = "aggregate:brandGP"
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
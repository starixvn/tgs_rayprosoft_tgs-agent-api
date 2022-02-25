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
    public class BrandGameProviderAggregateHandler : Handler<GenerateBrandGameProviderAggregateRequest, MessageResponse>
    {
        private readonly IBrandGameProviderAggregateAuditLogService _brandGameProviderAuditLogService;
        private readonly IBrandService _brandService;
        private readonly IMessageSender _messageSender;
        private readonly IRedisCacheManager _redisCacheManager;

        public BrandGameProviderAggregateHandler(
            IMessageSender messageSender,
            IBrandService brandService,
            IBrandGameProviderAggregateAuditLogService brandGameProviderAuditLogService,
            IRedisCacheManager redisCacheManager,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _brandService = brandService;
            _messageSender = messageSender;
            _brandGameProviderAuditLogService = brandGameProviderAuditLogService;
            _redisCacheManager = redisCacheManager;
        }

        public override async Task<MessageResponse> Handle(
            GenerateBrandGameProviderAggregateRequest request,
            CancellationToken cancellationToken)
        {
            var aggregate = await _redisCacheManager.GetSetAllMembersAsync("aggregate", "brandGP")
                .ConfigureAwait(false);

            if (aggregate.Length > 0)
            {
                return CreateInvalidResponse<MessageResponse>("waiting for report generation.");
            }

            var brandIds = request.BrandId is not null
                ? new[]
                {
                    request.BrandId.Value
                }
                : await _brandService.GetBrandQueries().Select(x => x.Id).ToArrayAsync(cancellationToken);

            var auditLogId =
                await _brandGameProviderAuditLogService.CreateBrandGameProviderAggregateAuditLogAsync(
                    new CreateBrandGameProviderAggregateAuditLogRequest(Guid.Parse(UserId), request.BrandId));

            await _redisCacheManager.SetAddAsync("aggregate",
                "brandGP",
                brandIds.Select(x => (RedisValue)x.ToString()).ToArray(),
                TimeSpan.FromMinutes(10)).ConfigureAwait(false);

            foreach (var brandId in brandIds)
            {
                var data = new
                {
                    request.StartDate,
                    BrandId = brandId,
                    AuditLogId = auditLogId,
                    Key = "aggregate:brandGP"
                };

                await _messageSender.PublishAsync("tgs.report.aggregate.brand.game.provider",
                        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)),
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            return CreateValidResponse<MessageResponse>();
        }
    }
}
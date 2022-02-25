using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.BackOffice.Application.Models.Aggregate;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IBrandGameProviderAuditLogService
    {
        ValueTask<int> CreateBrandGameProviderAggregateAuditLogAsync(
            CreateBrandGameProviderAggregateAuditLogRequest request);
    }

    public class BrandGameProviderAuditLogService : BaseRepositoryOperations<ITgsRepository>,
        IBrandGameProviderAuditLogService
    {
        public BrandGameProviderAuditLogService(ITgsRepository repository) : base(repository)
        {
        }

        async ValueTask<int> IBrandGameProviderAuditLogService.CreateBrandGameProviderAggregateAuditLogAsync(
            CreateBrandGameProviderAggregateAuditLogRequest request)
        {
            var log = new BrandGameProviderAggregateAuditLog
            {
                BrandId = request.BrandId,
                CreatedByUserId = request.UserId,
                CreatedOn = DateTimeOffset.UtcNow
            };

            await CreateEntityAsync(log);

            return log.Id;
        }
    }
}
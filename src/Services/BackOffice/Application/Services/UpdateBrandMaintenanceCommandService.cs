using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class UpdateBrandMaintenanceCommandService : ICommandService<BatchUpdateBrandMaintenanceCommand>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;
        private readonly IRedisCacheManager _cacheManager;

        public UpdateBrandMaintenanceCommandService(
            IBackOfficeRepository backOfficeRepository,
            IRedisCacheManager cacheManager)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public async ValueTask ExecuteAsync(BatchUpdateBrandMaintenanceCommand command, CancellationToken cancellationToken = default)
        {
            await _backOfficeRepository.UpdateBrandMaintenanceAsync(
                command.Ids,
                command.BeginTime,
                command.EndTime).ConfigureAwait(false);

            var brandMaintenanceInfos = await _backOfficeRepository
                .QueryBrandMaintenanceByIdsAsync(command.Ids)
                .ConfigureAwait(false);

            var data =
                brandMaintenanceInfos.Select(bmi => new
                {
                    bmi.BrandCode,
                    bmi.BeginTime,
                    bmi.EndTime,
                    bmi.Content
                })
                .ToArray();

            foreach (var bmi in data)
            {
                await _cacheManager.SafeCacheForeverAsync(RedisCacheType.BrandMaintenance, bmi.BrandCode, bmi);
            }
        }
    }
}

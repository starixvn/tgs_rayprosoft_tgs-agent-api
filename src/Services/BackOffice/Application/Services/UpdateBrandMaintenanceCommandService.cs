using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class UpdateBrandMaintenanceCommandService : ICommandService<BatchUpdateBrandMaintenanceCommand>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public UpdateBrandMaintenanceCommandService(
            IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }

        public async ValueTask ExecuteAsync(BatchUpdateBrandMaintenanceCommand command, CancellationToken cancellationToken = default)
        {
            await _backOfficeRepository.BatchUpdateOrAddBrandMaintenanceAsync(
                command.BrandCodes,
                command.BeginTime,
                command.EndTime).ConfigureAwait(false);
        }
    }
}

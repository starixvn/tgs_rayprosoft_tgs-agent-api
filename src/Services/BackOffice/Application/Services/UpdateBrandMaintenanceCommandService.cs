using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class UpdateBrandMaintenanceCommandService : ICommandService<UpdateBrandMaintenanceCommand>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public UpdateBrandMaintenanceCommandService(IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }

        public ValueTask ExecuteAsync(UpdateBrandMaintenanceCommand command, CancellationToken cancellationToken = default)
            => _backOfficeRepository.UpdateBrandMaintenanceAsync(command.Id, command.Content);
    }
}

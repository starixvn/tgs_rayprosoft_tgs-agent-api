using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class ChangeBannerOrderCommandService : ICommandService<ChangeBannerOrderCommand>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public ChangeBannerOrderCommandService(IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }

        public ValueTask ExecuteAsync(ChangeBannerOrderCommand command, CancellationToken cancellationToken = default)
            => _backOfficeRepository.ChangeBannerOrderAsync(
                command.BannerId,
                command.OrderNumber,
                command.Modifier);
    }
}

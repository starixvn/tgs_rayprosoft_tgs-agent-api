using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Services
{
    class ModifyBrandLogoInfoCommandService : ICommandService<ModifyBrandLogoInfoCommand>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public ModifyBrandLogoInfoCommandService(IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }
        public ValueTask ExecuteAsync(ModifyBrandLogoInfoCommand command, CancellationToken cancellationToken = default)
            => _backOfficeRepository.UpdateBrandLogoInfoAsync(
                new BrandLogoInfo(
                    command.BrandId,
                    command.DesktopLogoUrl,
                    command.MobileLogoUrl,
                    command.LoadingIconUrl),
                command.Modifier);
    }
}

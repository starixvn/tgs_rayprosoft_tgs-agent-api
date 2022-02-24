using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class CreateIdentityAuthenticationFormCommandService : ICommandService<CreateIdentityAuthenticationFormCommand>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IPlayerRepository _playerRepository;

        public CreateIdentityAuthenticationFormCommandService(
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(
            CreateIdentityAuthenticationFormCommand command,
            CancellationToken cancellationToken = default)
        {
            var brandInfo = await _brandRepository
                .GetBrandInfoAsync(command.BrandId)
                .ConfigureAwait(false);

            if (!brandInfo!.PlayerIdentityEnabled)
                throw new BrandEnablePlayerIdentityNotSetException();

            var data = JsonSerializer.Serialize(new PlayerAuthenticationFormData(command.FileId));

            var formId = await _playerRepository
                .CreatePlayerAuthenticationFormAsync(command.BrandId, command.PlayerCode, data)
                .ConfigureAwait(false);

            await _playerRepository
                .UpdatePlayerIdentityAsync(command.PlayerCode, formId)
                .ConfigureAwait(false);
        }
    }
}

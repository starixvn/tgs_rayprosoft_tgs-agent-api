using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Withdraw
{
    internal class CheckPlayerIdentityCommandService : ICommandService<CheckPlayerIdentityCommand>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IPlayerRepository _playerRepository;

        public CheckPlayerIdentityCommandService(IBrandRepository brandRepository, IPlayerRepository playerRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask ExecuteAsync(CheckPlayerIdentityCommand command, CancellationToken cancellationToken = default)
        {
            var brand = await _brandRepository
                .GetBrandInfoAsync(command.BrandId)
                .ConfigureAwait(false);

            if (!brand!.PlayerIdentityEnabled)
                return;

            var player = await _playerRepository
                .GetPlayerInfoAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (player == null)
                throw new PlayerNotFoundException("Player not found when withdraw check.");

            if (!player.IdentityAuthentication)
                throw new PlayerCanNotWithdrawException();
        }
    }
}

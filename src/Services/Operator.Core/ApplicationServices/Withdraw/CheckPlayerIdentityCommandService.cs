using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.BrandCertificate.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Withdraw
{
    internal class CheckPlayerIdentityCommandService : IGetService<CheckPlayerIdentityCommand, bool>
    {
        private readonly IBrandServiceClient _brandServiceClient;
        private readonly IPlayerRepository _playerRepository;

        public CheckPlayerIdentityCommandService(
            IBrandServiceClient brandServiceClient,
            IPlayerRepository playerRepository)
        {
            _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<bool> GetAsync(CheckPlayerIdentityCommand command, CancellationToken cancellationToken = default)
        {
            var brand = await _brandServiceClient
                .GetBrandInfoAsync(command.BrandId)
                .ConfigureAwait(false);

            if (!brand!.PlayerIdentityEnabled)
                return true;

            var player = await _playerRepository
                .GetPlayerInfoAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (player == null)
                throw new PlayerNotFoundException("Player not found when withdraw check.");

            if (!player.IdentityAuthentication)
                return false;

            return true;
        }
    }
}

using System;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class FirstDepositChecker : IPromotionChecker
    {
        private readonly IPlayerService _playerService;
        private readonly IPlayerDataRepository _playerDataRepository;

        public FirstDepositChecker(IPlayerService playerService, IPlayerDataRepository playerDataRepository)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _playerDataRepository = playerDataRepository ?? throw new ArgumentNullException(nameof(playerDataRepository));
        }

        public async ValueTask<bool> CheckAsync(string playerCode)
        {
            var isFirstDepositing = await _playerDataRepository
                .CheckPlayerFirstDepositingAsync(playerCode)
                .ConfigureAwait(false);

            if (isFirstDepositing)
                return false;

            var player = await _playerService
                .GetPlayerByPlayerCodeAsync(playerCode)
                .ConfigureAwait(false);

            if (player == null)
                return false;

            return player.FirstDepositOn == null;
        }

        public async ValueTask CommitTagAsync(string playerCode)
        {
            var depositTime = await _playerDataRepository
                .GetPlayerFirstDepositFlagTimeAsync(playerCode)
                .ConfigureAwait(false);

            if (depositTime == null)
                return;

            await _playerService
                .SetFirstDepositTimeAsync(playerCode, depositTime.Value)
                .ConfigureAwait(false);

            await _playerDataRepository
                .RemovePlayerFirstDepositFlagAsync(playerCode)
                .ConfigureAwait(false);
        }

        public ValueTask RollbackTagAsync(string playerCode)
            => _playerDataRepository.RemovePlayerFirstDepositFlagAsync(playerCode);

        public ValueTask TagPromotionTypeAsync(string playerCode)
            => _playerDataRepository.SetPlayerFirstDepositFlagAsync(playerCode);
    }
}

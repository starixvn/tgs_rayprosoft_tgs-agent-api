using System;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class FirstDepositChecker : IPromotionChecker
    {
        private readonly IPlayerDataRepository _playerDataRepository;
        private readonly IPlayerService _playerService;

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

        public ValueTask CommitTagAsync(string playerCode)
            => _playerDataRepository
                .RemovePlayerFirstDepositFlagAsync(playerCode);

        public ValueTask RollbackTagAsync(string playerCode)
            => _playerDataRepository.RemovePlayerFirstDepositFlagAsync(playerCode);

        public ValueTask TagPromotionTypeAsync(string playerCode)
            => _playerDataRepository.SetPlayerFirstDepositFlagAsync(playerCode);
    }
}

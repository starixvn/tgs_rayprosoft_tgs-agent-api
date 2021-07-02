using System;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class DailyFirstDepositChecker : IPromotionChecker
    {
        private readonly IPlayerDataRepository _playerDataRepository;

        public DailyFirstDepositChecker(IPlayerDataRepository playerDataRepository)
        {
            _playerDataRepository = playerDataRepository ?? throw new ArgumentNullException(nameof(playerDataRepository));
        }

        public ValueTask<bool> CheckAsync(string playerCode)
            => _playerDataRepository.CheckPlayerIsDailyFirstDepositAsync(playerCode);

        public ValueTask CommitTagAsync(string playerCode) => ValueTask.CompletedTask;

        public ValueTask RollbackTagAsync(string playerCode)
            => _playerDataRepository.RemovePlayerDailyDepositFlagAsync(playerCode);

        public ValueTask TagPromotionTypeAsync(string playerCode)
            => _playerDataRepository.SetPlayerDailyDepositFlagAsync(playerCode);
    }
}

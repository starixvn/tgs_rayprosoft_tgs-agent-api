using System;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class DailySecondDepositChecker : IPromotionChecker
    {
        private readonly IPlayerDataRepository _playerDataRepository;

        public DailySecondDepositChecker(IPlayerDataRepository playerDataRepository)
        {
            _playerDataRepository = playerDataRepository ?? throw new ArgumentNullException(nameof(playerDataRepository));
        }

        public async ValueTask<bool> CheckAsync(string playerCode)
        {
            return !await _playerDataRepository
                .CheckPlayerIsDailyFirstDepositAsync(playerCode)
                .ConfigureAwait(false);
        }

        public ValueTask CommitTagAsync(string playerCode) => ValueTask.CompletedTask;

        public ValueTask RollbackTagAsync(string playerCode) => ValueTask.CompletedTask;

        public ValueTask TagPromotionTypeAsync(string playerCode) => ValueTask.CompletedTask;
    }
}

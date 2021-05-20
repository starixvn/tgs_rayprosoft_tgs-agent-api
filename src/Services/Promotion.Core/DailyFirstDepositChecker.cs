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
    }
}

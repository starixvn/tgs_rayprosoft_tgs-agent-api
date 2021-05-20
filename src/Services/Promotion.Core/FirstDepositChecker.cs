using System;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class FirstDepositChecker : IPromotionChecker
    {
        private readonly IPlayerService _playerService;

        public FirstDepositChecker(IPlayerService playerService)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<bool> CheckAsync(string playerCode)
        {
            var player = await _playerService
                .GetPlayerByPlayerCodeAsync(playerCode)
                .ConfigureAwait(false);

            if (player == null)
                return false;

            return player.FirstDepositOn == null;
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{
    internal class PlayerRepository : IPlayerRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public PlayerRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<BalanceInfo> GetBalanceInfoAsync(string playerCode)
        {
            var playerWallet = await _tgsRepository.Wallets.AsAsyncEnumerable()
                .Where(wallet => wallet.PlayerCode == playerCode)
                .SingleAsync()
                .ConfigureAwait(false);

            return new BalanceInfo(playerWallet.Balance);
        }

        public async ValueTask<bool> GetPlayerAutoTransferFlagAsync(string playerCode)
        {
            var settingData = await _tgsRepository.PlayerGameSettings
                .FindAsync(playerCode)
                .ConfigureAwait(false);

            return settingData?.IsAutoTransferToGame ?? true;
        }

        public async ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode)
        {
            var playerData = await _tgsRepository.Players.FindAsync(playerCode).ConfigureAwait(false);

            return new PlayerInfo(
                playerData.PlayerCode,
                playerData.AccumulatedDeposit,
                playerData.IdentityAuthentication,
                playerData.PhoneAuthentication,
                playerData.VipId);
        }

        public async ValueTask TogglePlayerAutoTransferFlagAsync(string playerCode)
        {
            var settingData = await _tgsRepository.PlayerGameSettings
                .FindAsync(playerCode)
                .ConfigureAwait(false);

            if (settingData == null)
            {
                settingData = new PlayerGameSetting
                {
                    PlayerCode = playerCode,
                    IsAutoTransferToGame = false
                };

                await _tgsRepository.PlayerGameSettings
                    .AddAsync(settingData)
                    .ConfigureAwait(false);
            }

            settingData.IsAutoTransferToGame = !settingData.IsAutoTransferToGame;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

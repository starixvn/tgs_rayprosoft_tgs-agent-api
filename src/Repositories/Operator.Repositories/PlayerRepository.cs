using System;
using System.Threading.Tasks;
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
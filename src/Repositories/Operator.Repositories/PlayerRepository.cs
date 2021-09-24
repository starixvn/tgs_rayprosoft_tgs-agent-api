using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
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

        public async ValueTask PlayerLoginIpAsync(string playerCode, string ip)
        {
            var ipType = (int)IpType.LoginIp;
            var data = _tgsRepository.PlayerIpRecords
                .AsQueryable()
                .Where(x => x.PlayerCode == playerCode && x.IpType == ipType)
                .FirstOrDefault();

            if (data == null)
            {
                data = new PlayerIpRecord
                {
                    PlayerCode = playerCode,
                    IpType = ipType,
                    Ip = ip,
                    RecordOn = DateTimeOffset.UtcNow
                };
                _tgsRepository.PlayerIpRecords.Add(data);
            }
            else
            {
                data.Ip = ip;
                data.RecordOn = DateTimeOffset.UtcNow;
                _tgsRepository.PlayerIpRecords.Update(data);
            }

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask PlayerRegisterIpAsync(string playerCode, string ip, DateTimeOffset registeredOn)
        {
            var datas = new PlayerIpRecord[]
            {
                new()
                {
                    PlayerCode = playerCode,
                    IpType = (int)IpType.RegisterIp,
                    Ip = ip,
                    RecordOn = registeredOn
                },
                new()
                {
                    PlayerCode = playerCode,
                    IpType = (int)IpType.LoginIp,
                    Ip = ip,
                    RecordOn = registeredOn
                }
            };
            await _tgsRepository.PlayerIpRecords.AddRangeAsync(datas);
            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask SetPlayerPhoneAsync(string playerCode, string areaCode, string phone)
        {
            var playerData = await _tgsRepository.Players
                .FindAsync(playerCode)
                .ConfigureAwait(false);

            if (playerData == null)
                return;

            playerData.AreaCode = areaCode;
            playerData.Phone = phone;
            playerData.PhoneAuthentication = true;

            playerData.UpdatedOn = DateTimeOffset.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
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

        public async ValueTask<PlayerPasswordInfo?> GetPlayerPasswordInfoByUsernameAsync(Guid brandId, string username)
        {
            var playerData = await _tgsRepository.Players
                .FirstOrDefaultAsync(p => p.BrandId == brandId &&
                    p.Username.ToLower() == username.ToLower())
                .ConfigureAwait(false);

            if (playerData == null)
            {
                return null;
            }

            return new PlayerPasswordInfo(
                playerData.PlayerCode,
                playerData.Username,
                playerData.Salt,
                playerData.Password);
        }
    }
}

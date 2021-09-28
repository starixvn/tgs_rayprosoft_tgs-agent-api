using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public interface IPlayerRepository
    {
        ValueTask<bool> GetPlayerAutoTransferFlagAsync(string playerCode);

        ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode);

        ValueTask<PlayerInfo?> GetPlayerInfoByUserNameAsync(Guid brandId, string userName);

        ValueTask<PasswordInfo> GetPlayerLoginPasswordInfoAsync(string playerCode);

        ValueTask PlayerLoginIpAsync(string playerCode, string ip);

        ValueTask PlayerRegisterIpAsync(string playerCode, string ip, DateTimeOffset registeredOn);

        ValueTask TogglePlayerAutoTransferFlagAsync(string playerCode);

        ValueTask UpdatePlayerStatusAsync(string playerCode, PlayerStatus playerStatus);
    }
}
using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public interface IPlayerRepository
    {
        ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode);

        ValueTask TogglePlayerAutoTransferFlagAsync(string playerCode);

        ValueTask<bool> GetPlayerAutoTransferFlagAsync(string playerCode);

        ValueTask PlayerRegisterIpAsync(string playerCode, string ip, DateTimeOffset registeredOn);

        ValueTask PlayerLoginIpAsync(string playerCode, string ip);

        ValueTask<PlayerPasswordInfo?> GetPlayerPasswordInfoByUsernameAsync(Guid brandId, string username);

        ValueTask UpdatePlayerStatusAsync(string playerCode, PlayerStatus playerStatus);
    }
}
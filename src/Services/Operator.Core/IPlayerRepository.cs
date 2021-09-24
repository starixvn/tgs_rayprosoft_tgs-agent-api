using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IPlayerRepository
    {
        ValueTask<bool> GetPlayerAutoTransferFlagAsync(string playerCode);

        ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode);

        ValueTask PlayerLoginIpAsync(string playerCode, string ip);

        ValueTask PlayerRegisterIpAsync(string playerCode, string ip, DateTimeOffset registeredOn);
        ValueTask<PasswordInfo> GetPlayerLoginPasswordInfoAsync(string playerCode);

        ValueTask PlayerLoginIpAsync(string playerCode, string ip);

        ValueTask SetPlayerPhoneAsync(string playerCode, string areaCode, string phone);

        ValueTask TogglePlayerAutoTransferFlagAsync(string playerCode);
    }
}

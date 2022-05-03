using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public interface IPlayerRepository
    {
        ValueTask<LoginRecordInfo> AddPlayerLoginRecordAsync(Guid brandId, string playerCode, PlatformType platformType, string ipAddress);

        ValueTask<Guid> CreatePlayerAuthenticationFormAsync(Guid brandId, string playerCode, string data);

        ValueTask DeauthorizePlayerAsync(Guid loginRecordId);

        ValueTask<PlayerInfo?> FindPlayerInfoByPhoneAsync(Guid brandId, string areaCode, string phone);

        ValueTask<bool> GetPlayerAutoTransferFlagAsync(string playerCode);

        ValueTask<PlayerIdentityFormInfo> GetPlayerIdentityFormQueryAsync(string playerCode);

        ValueTask<PlayerInfo?> GetPlayerInfoAsync(string playerCode);

        ValueTask<PlayerInfo?> GetPlayerInfoByUserNameAsync(Guid brandId, string userName);

        ValueTask<PasswordInfo> GetPlayerLoginPasswordInfoAsync(string playerCode);

        ValueTask<LoginRecordInfo?> GetPlayerLoginRecordAsync(Guid loginRecordId);

        ValueTask PlayerLoginIpAsync(string playerCode, string ip, string host);

        ValueTask PlayerRegisterIpAsync(string playerCode, string ip, string? host, DateTimeOffset registeredOn);

        ValueTask SetPlayerPhoneAsync(string playerCode, string areaCode, string phone);

        ValueTask TogglePlayerAutoTransferFlagAsync(string playerCode);

        ValueTask UpdatePlayerIdentityAsync(string playerCode, Guid identityAuthenticationFormId);

        ValueTask UpdatePlayerStatusAsync(string playerCode, PlayerStatus playerStatus);
    }
}

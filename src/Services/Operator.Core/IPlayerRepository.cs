using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IPlayerRepository
    {
        ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode);

        ValueTask TogglePlayerAutoTransferFlagAsync(string playerCode);

        ValueTask<bool> GetPlayerAutoTransferFlagAsync(string playerCode);

        ValueTask PlayerRegisterIpAsync(string playerCode, string ip);

        ValueTask PlayerLoginIpAsync(string playerCode, string ip);
    }
}

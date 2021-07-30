using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IPlayerRepository
    {
        ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode);

        ValueTask TogglePlayerAutoTransferFlagAsync(string playerCode);

        ValueTask<bool> GetPlayerAutoTransferFlagAsync(string playerCode);
    }
}

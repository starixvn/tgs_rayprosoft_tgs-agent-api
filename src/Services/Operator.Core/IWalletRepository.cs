using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IWalletRepository
    {
        ValueTask<BalanceInfo> GetBalanceInfoAsync(string playerCode);

        ValueTask<PlayerInfo> GetPlayerInfoAsync(string playerCode);
    }
}

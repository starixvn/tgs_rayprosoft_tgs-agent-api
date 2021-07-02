using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    public interface IPlayerDataRepository
    {
        ValueTask<bool> CheckPlayerFirstDepositingAsync(string playerCode);

        ValueTask<bool> CheckPlayerIsDailyFirstDepositAsync(string playerCode);

        ValueTask RemovePlayerDailyDepositFlagAsync(string playerCode);

        ValueTask RemovePlayerFirstDepositFlagAsync(string playerCode);

        ValueTask SetPlayerDailyDepositFlagAsync(string playerCode);

        ValueTask SetPlayerFirstDepositFlagAsync(string playerCode);
    }
}

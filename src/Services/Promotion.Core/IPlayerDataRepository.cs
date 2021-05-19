using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    public interface IPlayerDataRepository
    {
        ValueTask<bool> CheckPlayerIsDailyFirstDepositAsync(string playerCode);

        ValueTask SetPlayerDepositAsync(string playerCode);
    }
}

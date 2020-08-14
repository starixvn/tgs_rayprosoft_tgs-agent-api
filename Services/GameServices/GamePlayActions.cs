using System.Threading.Tasks;
using TGS.Core.Models.Services;

namespace TGS.Services.GameServices
{
    public interface IGamePlayActions
    {
        Task ValidatePlayerAsync();

        Task GetBalanceAsync(string gameProviderCode, string playerCode);

        Task<BetResult> LockAmountAsync(BetData data);

        Task<BetResult> PlaceBetAsync(BetData data);

        Task<BetResult> SettleBetAsync(BetData data);

        Task<BetResult> CancelTransactionAsync(BetData data);
    }

    public sealed class GamePlayActions
    {
    }
}

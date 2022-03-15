using System.Threading.Tasks;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations
{
    public interface IGameActionDataOperations
    {
        ValueTask<BatchBetResult> CancelBatchAsync(BatchBetData data);

        ValueTask<BetResult> CancelTransactionAsync(BetData data);

        ValueTask<BetResult> LockAmountAsync(BetData data);

        ValueTask<BetResult> PlaceAndSettleBetAsync(BetData data);

        ValueTask<BetResult> PlaceBetAsync(BetData data);

        ValueTask<BatchBetResult> SettleBatchAsync(BatchBetData data);

        ValueTask<BetResult> SettleBetAsync(BetData data);
    }
}

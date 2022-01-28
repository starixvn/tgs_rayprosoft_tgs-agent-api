using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations
{
    public interface IGameActionDataOperations
    {
        ValueTask<BetResult> LockAmountAsync(BetData data);

        ValueTask<BetResult> PlaceBetAsync(BetData data);

        ValueTask<BetResult> SettleBetAsync(BetData data);

        ValueTask<BetResult> CancelTransactionAsync(BetData data);

        ValueTask<BetResult> PlaceAndSettleBetAsync(BetData data);

        ValueTask<BatchBetResult> SettleBatchAsync(BatchBetData data);

        ValueTask<BatchBetResult> CancelBatchAsync(BatchBetData data);
    }
}

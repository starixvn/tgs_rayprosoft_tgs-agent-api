using System.Threading.Tasks;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations
{
    public partial interface IGamePlayActions
    {
        ValueTask<BatchBetResult> SettleBatchAsync(BatchBetData data);

        ValueTask<BatchBetResult> CancelBatchAsync(BatchBetData data);
    }

    public sealed partial class GamePlayActions
    {
        public ValueTask<BatchBetResult> SettleBatchAsync(BatchBetData data)
            => _operations.SettleBatchAsync(data);

        public ValueTask<BatchBetResult> CancelBatchAsync(BatchBetData data)
            => _operations.CancelBatchAsync(data);
    }
}
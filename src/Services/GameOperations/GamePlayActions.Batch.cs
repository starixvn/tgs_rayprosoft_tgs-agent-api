using System.Threading.Tasks;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations
{
    public partial interface IGamePlayActions
    {
        Task<BatchBetResult> SettleBatchAsync(BatchBetData data);

        Task<BatchBetResult> CancelBatchAsync(BatchBetData data);
    }

    public sealed partial class GamePlayActions
    {
        Task<BatchBetResult> IGamePlayActions.SettleBatchAsync(BatchBetData data) =>
            _operations.SettleBatchAsync(data);

        Task<BatchBetResult> IGamePlayActions.CancelBatchAsync(BatchBetData data) =>
            _operations.CancelBatchAsync(data);
    }
}
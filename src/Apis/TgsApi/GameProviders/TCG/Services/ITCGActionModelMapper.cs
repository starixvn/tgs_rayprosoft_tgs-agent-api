using System;
using System.Threading.Tasks;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public interface ITCGActionModelMapper
    {
        ValueTask<BetData[]> BetAsync(Guid gameProviderId, CachedGame cachedGame, TCGDebitRequest source);

        ValueTask<BatchBetData> CancelReserveAsync(Guid gameProviderId, TCGCancelReserveRequest source);

        ValueTask<QueryResult> CheckTransactionStatusAsync(Guid gameProviderId, string player, string externalTransactionId);

        ValueTask<BetData> CloseAsync(TCGCloseRequest source);

        ValueTask<BatchBetData> ConfirmReserveAsync(Guid gameProviderId, CachedGame cachedGame, TCGConfirmReserveRequest source);

        ValueTask<BetData[]> ReserveMoneyAsync(Guid gameProviderId, CachedGame cachedGame, TCGReserveMoneyRequest source);

        ValueTask<BatchBetData> SettleAsync(Guid gameProviderId, CachedGame cachedGame, TCGCreditRequest source);
    }
}

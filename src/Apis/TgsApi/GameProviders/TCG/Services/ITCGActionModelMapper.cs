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
        ValueTask<BetData[]> BetAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGDebitRequest source);

        ValueTask<BatchBetData> CancelReserveAsync(Guid gameProviderId, string gameProviderCode, TCGCancelReserveRequest source);

        ValueTask<QueryResult> CheckTransactionStatusAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, string player, string externalTransactionId);

        ValueTask<BatchBetData> ConfirmReserveAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGConfirmReserveRequest source);

        ValueTask<BetData[]> ReserveMoneyAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGReserveMoneyRequest source);

        ValueTask<BatchBetData> SettleAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGCreditRequest source);
    }
}

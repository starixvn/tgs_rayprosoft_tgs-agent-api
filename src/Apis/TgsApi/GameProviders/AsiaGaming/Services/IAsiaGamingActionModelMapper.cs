using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public interface IAsiaGamingActionModelMapper
    {
        ValueTask<BetData> CancelTransactionRequestAsync(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            bool isLockGame,
            OppositeBetSetting[] oppositeBetSettings);

        BetData LockAmountRequest(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r);

        ValueTask<BetData> PlaceBetForLockGameRequestAsync(
            string playerCode,
            GameProviderConfigurationData data,
            BetResponseRequest r,
            GameActionExternalIds lockGameActionExternalIds,
            OppositeBetSetting[] oppositeBetSettings);

        ValueTask<BetData> PlaceBetRequestAsync(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            OppositeBetSetting[] oppositeBetSettings);

        TransferResponse Response(GameApiError error);

        TransferResponse Response(GameApiError error, GetBalanceResult result);

        TransferResponse Response(BetResult result);

        ValueTask<(BetData BetData, bool RoundClosed)> SettleBetRequestAsync(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            TransactionType transactionType,
            bool isLockGame,
            OppositeBetSetting[] oppositeBetSettings);
    }
}

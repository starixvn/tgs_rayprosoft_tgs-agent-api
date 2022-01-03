using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public interface IAsiaGamingActionModelMapper
    {
        BetData CancelTransactionRequest(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            TransactionType transactionType,
            bool isLockGame,
            OppositeBetSetting[] oppositeBetSettings);

        BetData LockAmountRequest(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r);

        BetData PlaceBetRequest(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            OppositeBetSetting[] oppositeBetSettings);

        BetData PlaceBetRequest(string playerCode,
            GameProviderConfigurationData data,
            BetResponseRequest r,
            GameActionExternalIds lockGameActionExternalIds,
            OppositeBetSetting[] oppositeBetSettings);
        
        TransferResponse Response(GameApiError error);

        TransferResponse Response(GameApiError error, GetBalanceResult result);

        TransferResponse Response(BetResult result);

        BetData SettleBetRequest(
            string playerCode,
            GameProviderConfigurationData data,
            PostTransferRequest r,
            TransactionType transactionType,
            bool isLockGame,
            OppositeBetSetting[] oppositeBetSettings);
    }
}
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.WMCasino.Common;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoActionModelMapper
    {
        BetData LockAmountRequest(string playerCode, 
            GameProviderConfigurationData data, 
            WalletTransferRequest request);

        BetData CancelLockAmountRequest(
            string playerCode, 
            GameProviderConfigurationData data,
            CancelWalletTransferRequest request);

        BetData PlaceAndSettleBetRequest(
            GameProviderConfigurationData data, 
            string playerCode, 
            GameBetResult[] results,
            GameActionExternalIds[] externalIds, 
            bool fromSchedule);

        (BetData CancelBetData, BetData SettleBetData) ResettleRequest(
            GameProviderConfigurationData data, 
            string playerCode, GameBetResult[] betResults,
            GameActionExternalIds[] externalIds);

        BetData VoidRoundRequest(
            GameProviderConfigurationData data, 
            string playerCode, 
            GameBetResult[] betResults,
            GameActionExternalIds[] externalIds);

        GetBalanceResponse Response(
            string playerCode, 
            GetBalanceResult r);

        WalletTransferResponse Response(
            WalletTransferRequest request,
            decimal balance);

        CancelWalletTransferResponse Response(
            CancelWalletTransferRequest request,
            decimal balance);

        ErrorResponse Response(
            GameApiError error, 
            string message);
    }
}
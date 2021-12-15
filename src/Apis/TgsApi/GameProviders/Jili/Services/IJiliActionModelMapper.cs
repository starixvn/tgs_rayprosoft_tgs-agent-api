using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public interface IJiliActionModelMapper
    {
        JiliAuthorizePlayerResponse AuthorizePlayerResponse(
            string playerCode,
            PlayerValidationResult result);

        BetData BetAndSettleRequest(
            string playerCode,
            GameProviderConfigurationData data,
            JiliPlaceBetAndSettleRequest request);

        JiliPlaceBetAndSettleResponse BetAndSettleResponse(
            string jiliPlayerId,
            BetResult result);

        JiliCancelTransactionResponse FailCancelTransactionResponse(
            int errorCode,
            string errorMessage,
            string jiliPlayerId,
            GetBalanceResult balanceResult);

        BetData FreeBetRequest(
            string playerCode,
            GameProviderConfigurationData data,
            JiliPlaceBetAndSettleRequest request);

        JiliCancelTransactionResponse SuccessCancelTransactionResponse(
            string jiliPlayerId,
            GetBalanceResult balanceResult,
            Guid cancelledGameActionId);

        PlayerValidationData ValidateRequest(
                    string playerCode,
            PlatformType platformType);
    }
}

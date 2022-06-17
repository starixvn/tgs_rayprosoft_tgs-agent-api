using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

public interface IJiliActionModelMapper
{
    JiliAuthorizePlayerResponse AuthorizePlayerResponse(
        string playerCode,
        PlayerValidationResult result);

    (BetData BetData, string ExternalRoundId) BetAndSettleRequest(
        string playerCode,
        GameProviderConfigurationData data,
        JiliPlaceBetAndSettleRequest request);

    BetData CancelFreeBetRequest(BetData freeBetData);

    BetData CancelPlaceAndSettleBetRequest(BetData placeAndSettleBetData);

    JiliPlaceBetAndSettleResponse FailBetAndSettleResponse(
                int errorCode,
        string errorMessage,
        string jiliPlayerId,
        BetResult result);

    JiliCancelTransactionResponse FailCancelTransactionResponse(
        int errorCode,
        string errorMessage,
        string jiliPlayerId,
        GetBalanceResult balanceResult);

    (BetData BetData, string ExternalRoundId) FreeBetRequest(
        string playerCode,
        GameProviderConfigurationData data,
        JiliPlaceBetAndSettleRequest request);

    JiliPlaceBetAndSettleResponse SuccessBetAndSettleResponse(
        string jiliPlayerId,
        BetResult result);

    JiliCancelTransactionResponse SuccessCancelTransactionResponse(
        string jiliPlayerId,
        GetBalanceResult balanceResult,
        Guid cancelledGameActionId);

    PlayerValidationData ValidateRequest(
        string playerCode,
        PlatformType platformType);
}

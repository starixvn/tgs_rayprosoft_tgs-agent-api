using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public interface IJiliActionModelMapper
    {
        JiliAuthorizePlayerResponse AuthorizePlayerResponse(string playerCode, PlayerValidationResult result);

        BetData BetAndSettleRequest(string playerCode, GameProviderConfigurationData data, JiliPlaceBetAndSettleRequest request);

        JiliPlaceBetAndSettleResponse BetAndSettleResponse(string jiliPlayerId, BetResult result);

        BetData FreeBetRequest(string playerCode, GameProviderConfigurationData data, JiliPlaceBetAndSettleRequest request);

        PlayerValidationData ValidateRequest(string playerCode, PlatformType platformType);
    }
}

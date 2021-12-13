using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public interface IJiliActionModelMapper
    {
        JiliAuthorizePlayerResponse AuthorizePlayerResponse(string playerCode, PlayerValidationResult result);

        PlayerValidationData ValidateRequest(string playerCode, PlatformType platformType);
    }
}
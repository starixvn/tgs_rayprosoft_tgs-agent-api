using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public interface IPragmaticPlayActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);
        AuthorizePlayerResponse Response(string playerCode, string token, PlayerValidationResult validationResult);
    }

    public sealed class PragmaticPlayActionModelMapper : IPragmaticPlayActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.PragmaticPlay;

        PlayerValidationData IPragmaticPlayActionModelMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        AuthorizePlayerResponse IPragmaticPlayActionModelMapper.Response(string playerCode, string token,
            PlayerValidationResult validationResult) =>
            new()
            {
                PlayerCode = playerCode,
                Currency = validationResult.GameProviderCurrencyCode,
                Balance = validationResult.Balance,
                Bonus = 0,
                Token = token
            };
    }
}
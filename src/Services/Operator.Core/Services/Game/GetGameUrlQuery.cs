using System.Text.Json;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.Game
{
    public record GetGameUrlQuery(
        PlayerTokenData PlayerTokenData,
        string GameProviderCode,
        int GameId,
        string LanguageCode,
        JsonDocument? ExtendedData);
}
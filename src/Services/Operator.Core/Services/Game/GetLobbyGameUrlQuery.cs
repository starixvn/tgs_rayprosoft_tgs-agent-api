using System.Text.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.Game
{
    public record GetLobbyGameUrlQuery(
        string GameProviderCode,
        PlayerTokenData PlayerTokenData,
        GameType GameType,
        string LanguageCode,
        JsonDocument? Extends);
}
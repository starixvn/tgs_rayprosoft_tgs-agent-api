using System;
using System.Text.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.Game
{
    public record GetLobbyGameUrlQuery(
        Guid BrandId,
        string GameProviderCode,
        PlayerTokenData PlayerTokenData,
        PlayerType PlayerType,
        GameType GameType,
        string LanguageCode,
        JsonDocument? Extends);
}
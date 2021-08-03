using System;
using System.Text.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.Game
{
    public record GetGameUrlQuery(
        Guid BrandId,
        PlayerTokenData PlayerTokenData,
        PlayerType PlayerType,
        string SubGameProviderCode,
        int GameId,
        string LanguageCode,
        JsonDocument? ExtendedData);
}
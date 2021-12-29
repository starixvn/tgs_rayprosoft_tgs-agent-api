using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.ApplicationServices.Game
{
    public record GetGamesQuery(
        Guid BrandId,
        string SubGameProviderCode,
        PlayerType PlayerType,
        PlatformType PlatformType,
        GameType[] Types);
}

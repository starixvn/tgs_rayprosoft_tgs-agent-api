using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;

namespace TGS.Services.Operator.Services.Game
{
    public record GetAllGamesByGameTypeQuery(
        Guid BrandId,
        PlayerTokenData? PlayerTokenData,
        PlatformType PlatformType,
        GameType[] Types);
}
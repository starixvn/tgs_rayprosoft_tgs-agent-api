using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Services.Game
{
    public record GetAllGamesByGameTypeQuery(
        Guid BrandId,
        PlayerType PlayerType,
        PlatformType PlatformType,
        GameType[] Types);
}

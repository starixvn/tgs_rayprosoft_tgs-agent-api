using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Services.Game
{
    public record ListFavoriteGameIdsQuery(Guid BrandId, string PlayerCode, GameType GameType);
}

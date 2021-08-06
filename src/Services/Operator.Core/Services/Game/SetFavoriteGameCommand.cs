using System;

namespace TGS.Services.Operator.Services.Game
{
    public record SetFavoriteGameCommand(
        Guid BrandId,
        string PlayerCode,
        int GameId);
}

using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record GameTypeAndStatusDataInfo(GameType Type, GameStatus Status, string SubGameProviderCode);
}
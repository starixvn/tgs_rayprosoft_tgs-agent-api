using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record GameTypeAndStatusInfo(GameType Type, GameStatus Status, string SubGameProviderCode);
}
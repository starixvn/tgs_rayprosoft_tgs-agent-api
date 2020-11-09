using System.Linq;
using TGS.Core.Models.Enums;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Core.Models.Extensions
{
    public static class GameActionTypeExtensions
    {
        public static bool IsOneOf(this GameActionType type, params GameActionType[] types)
        {
            return types.Any(t => t == type);
        }

        public static bool IsNotAllOf(this GameActionType type, params GameActionType[] types)
        {
            return types.All(t => t != type);
        }

        public static bool IsSettleBet(this GameActionType type)
        {
            return type.IsOneOf(WinBet, LoseBet, TieBet, FreeBet);
        }
    }
}

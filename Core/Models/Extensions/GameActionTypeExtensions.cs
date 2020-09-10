using System.Linq;
using TGS.Core.Models.Enums;

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
    }
}

using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Extensions
{
    public static class PlayerTypesExtensions
    {
        public static bool IsTestPlayer(this PlayerType type)
        {
            return (PlayerType.TestPlayers & type) == type;
        }
    }
}

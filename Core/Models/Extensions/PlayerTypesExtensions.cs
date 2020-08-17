using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Extensions
{
    public static class PlayerTypesExtensions
    {
        public static bool IsTestPlayer(this PlayerTypes type)
        {
            return (PlayerTypes.TestPlayers & type) == type;
        }
    }
}

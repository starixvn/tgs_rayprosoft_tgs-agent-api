using TGS.Core.Models.Enums;

namespace TGS.Core.Models
{
    public static class ValidStatuses
    {
        public static readonly Status[] ByDefault = { Status.Active };
        public static readonly Status[] OnClosingBet = { Status.Active, Status.Maintenance };

        public static readonly GameStatus[] ForTesting = { GameStatus.Active, GameStatus.Testing };

        public static Status[] GetStatus(PlayerTypes playerType, GameActionType type = GameActionType.PlaceBet)
        { 
            if (type != GameActionType.PlaceBet)
            {
                return OnClosingBet;
            }

            return playerType switch
            {
                PlayerTypes.TgsTestPlayer => OnClosingBet,
                _ => ByDefault
            };
        }
    }
}

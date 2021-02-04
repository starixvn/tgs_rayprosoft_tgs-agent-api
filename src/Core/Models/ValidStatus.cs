using TGS.Core.Models.Enums;

namespace TGS.Core.Models
{
    public interface IValidStatus<out T> where T : struct
    {
        T[] GetStatus(PlayerType playerType, GameActionType type = GameActionType.PlaceBet);
    }

    public class ValidStatus : IValidStatus<Status>
    {
        public static Status[] Default => new[] { Status.Active };
        public static Status[] ClosingBet => new[] { Status.Active, Status.Maintenance };

        Status[] IValidStatus<Status>.GetStatus(PlayerType playerType, GameActionType type)
        {
            if (type != GameActionType.PlaceBet)
            {
                return ClosingBet;
            }

            return playerType switch
            {
                PlayerType.TestPlayer => ClosingBet,
                _ => Default
            };
        }
    }

    public class ValidGameStatus : IValidStatus<GameStatus>
    {
        public static GameStatus[] Default => new[] { GameStatus.Active };
        public static GameStatus[] ClosingBet => new[] { GameStatus.Active, GameStatus.Maintenance, GameStatus.Testing };

        GameStatus[] IValidStatus<GameStatus>.GetStatus(PlayerType playerType, GameActionType type)
        {
            if (type != GameActionType.PlaceBet)
            {
                return ClosingBet;
            }

            return playerType switch
            {
                PlayerType.TestPlayer => ClosingBet,
                _ => Default
            };
        }
    }
}

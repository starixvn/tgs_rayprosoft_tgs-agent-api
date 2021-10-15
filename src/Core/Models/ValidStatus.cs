using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;

namespace TGS.Core.Models
{
    public interface IValidStatus<out T> where T : struct
    {
        T[] GetStatus(PlayerType playerType, GameActionType type = GameActionType.PlaceBet);
    }

    public class ValidStatus : IValidStatus<Status>
    {
        public static Status[] ClosingBet => new[]
        {
            Status.Active,
            Status.Maintenance
        };

        public static Status[] Default => new[]
        {
            Status.Active
        };

        Status[] IValidStatus<Status>.GetStatus(PlayerType playerType, GameActionType type)
        {
            if (type.IsNotAllOf(GameActionType.LockAmount, GameActionType.PlaceBet))
            {
                return ClosingBet;
            }

            return playerType switch
            {
                PlayerType.TestPlayer => ClosingBet,
                _                     => Default
            };
        }
    }

    public class ValidGameStatus : IValidStatus<GameStatus>
    {
        public static GameStatus[] ClosingBet => new[]
        {
            GameStatus.Active,
            GameStatus.Maintenance,
            GameStatus.Testing
        };

        public static GameStatus[] Default => new[]
        {
            GameStatus.Active
        };

        GameStatus[] IValidStatus<GameStatus>.GetStatus(PlayerType playerType, GameActionType type)
        {
            if (type.IsNotAllOf(GameActionType.LockAmount, GameActionType.PlaceBet))
            {
                return ClosingBet;
            }

            return playerType switch
            {
                PlayerType.TestPlayer => ClosingBet,
                _                     => Default
            };
        }
    }
}
using NUnit.Framework;
using TGS.Core.Models.Enums;

namespace TGS.UnitTests.Operations;

[TestFixture]
public class GamePlayActionsTests
{
    [TestCase(GameStatus.Active, GameStatus.Active, GameStatus.Active)]
    [TestCase(GameStatus.Maintenance, GameStatus.Active, GameStatus.Maintenance)]
    [TestCase(GameStatus.Active, GameStatus.Maintenance, GameStatus.Maintenance)]
    [TestCase(GameStatus.Inactive, GameStatus.Maintenance, GameStatus.Inactive)]
    [TestCase(GameStatus.Active, GameStatus.Inactive, GameStatus.Inactive)]
    [TestCase(GameStatus.Active, GameStatus.Testing, GameStatus.Testing)]
    [TestCase(GameStatus.Maintenance, GameStatus.Testing, GameStatus.Testing)]
    public void Assert_GameStatus_Of_BrandGame_And_Game(
        GameStatus brandGameStatus, GameStatus gameStatus, GameStatus expected)
    {
        var actual = GetGameStatus(brandGameStatus, gameStatus);

        Assert.AreEqual(expected, actual);
    }

    private GameStatus GetGameStatus(GameStatus brandGameStatus, GameStatus gameStatus)
    {
        if (brandGameStatus == GameStatus.Inactive || gameStatus == GameStatus.Inactive)
        {
            return GameStatus.Inactive;
        }

        if (brandGameStatus == GameStatus.Testing || gameStatus == GameStatus.Testing)
        {
            return GameStatus.Testing;
        }

        if (brandGameStatus == GameStatus.Maintenance || gameStatus == GameStatus.Maintenance)
        {
            return GameStatus.Maintenance;
        }

        return GameStatus.Active;
    }
}
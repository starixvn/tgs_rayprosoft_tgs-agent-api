using FluentAssertions;
using NUnit.Framework;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;

namespace TGS.UnitTests.Services.WMCasino;

[TestFixture]
public class WmCasinoUtilityTests
{
    [Test]
    public void Can_Get_Correct_Round_Id_As_Game_Number_Should_Start_With_Game_Id()
    {
        var sut = new WMCasinoUtility();

        var gameId = "101";
        var gameNumber = "101_112782375_21";
        var expected = "112782375_21";

        var actual = sut.GetExternalRoundIdByGameNumber(gameNumber, gameId);

        actual.Should().Be(expected);
    }

    [Test]
    public void Can_Get_Correct_Round_Id_As_Game_Number_Should_Start_With_Game_Id_And_Game_Number_Middle_Part_Contains_Game_Id()
    {
        var sut = new WMCasinoUtility();

        var gameId = "101";
        var gameNumber = "101_112782101_21";
        var expected = "112782101_21";

        var actual = sut.GetExternalRoundIdByGameNumber(gameNumber, gameId);

        actual.Should().Be(expected);
    }

    [Test]
    public void Can_Get_Correct_Round_Id_As_Game_Number_Should_Start_With_Game_Id_And_Game_Number_Suffix_Equals_Cancel()
    {
        var sut = new WMCasinoUtility();

        var gameId = "101";
        var gameNumber = "101_112782321_101_cancel";
        var expected = "112782321_101";

        var actual = sut.GetExternalRoundIdByGameNumber(gameNumber, gameId);

        actual.Should().Be(expected);
    }

    [Test]
    public void Can_Get_Correct_Round_Id_As_Game_Number_Should_Start_With_Game_Id_And_Game_Number_Suffix_Equals_Game_Id()
    {
        var sut = new WMCasinoUtility();

        var gameId = "101";
        var gameNumber = "101_112782321_101";
        var expected = "112782321_101";

        var actual = sut.GetExternalRoundIdByGameNumber(gameNumber, gameId);

        actual.Should().Be(expected);
    }
}

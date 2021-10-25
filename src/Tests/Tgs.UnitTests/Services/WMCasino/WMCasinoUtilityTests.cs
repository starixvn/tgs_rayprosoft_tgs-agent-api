using FluentAssertions;
using NUnit.Framework;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;

namespace TGS.UnitTests.Services.WMCasino
{
    [TestFixture]
    public class WmCasinoUtilityTests
    {
        [Test]
        public void Can_Get_Correct_Round_Id()
        {
            var sut = new WMCasinoUtility();

            var gameId = "101";
            var gameNumber = "101_112782375_21";
            var expected = "112782375_21";

            var actual = sut.GetExternalRoundIdByGameNumber(gameNumber, gameId);

            actual.Should().Be(expected);
        }

        [Test]
        public void Can_Get_Correct_Round_Id_When_Round_Id_Contains_Game_Id()
        {
            var sut = new WMCasinoUtility();

            var gameId = "101";
            var gameNumber = "101_112782101_21";
            var expected = "112782101_21";

            var actual = sut.GetExternalRoundIdByGameNumber(gameNumber, gameId);

            actual.Should().Be(expected);
        }
    }
}
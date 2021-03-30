using Moq;
using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Operator.StationLatters;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class StationLatterTests
    {
        [Test]
        public void Player_Can_List_Station_Latters()
        {
            var moqStationLatterRepository = new Mock<IStationLatterRepository>();

            var sut = new ListStationLattersQueryService(moqStationLatterRepository.Object);

            var playerCode = "ThisIsPlayerCode";

            var actual = sut.QueryAsync(new ListStationLattersQuery(playerCode));

            moqStationLatterRepository.Verify(
                fake => fake.QueryStationLattersAsync(It.Is<string>(x => x == playerCode)),
                Times.Exactly(1));
        }
    }
}

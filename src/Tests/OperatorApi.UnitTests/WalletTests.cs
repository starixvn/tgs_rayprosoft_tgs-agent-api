using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Operator.Wallet;
using ExpectedObjects;
using Moq;
using System.Threading.Tasks;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class WalletTests
    {
        [Test]
        public async Task Player_Can_Get_Balance_Info()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new GetBalanceQueryService(moqWalletRepository.Object);

            var playerCode = "ThisIsPlayerCode";
            var expected = new BalanceInfo(10000M).ToExpectedObject();

            moqWalletRepository
                .Setup(fake => fake.GetBalanceInfoAsync(It.Is<string>(x => x == playerCode)))
                .Returns(ValueTask.FromResult(new BalanceInfo(10000M)));

            var actual = await sut.GetAsync(new GetBalanceQuery(playerCode));

            expected.ShouldMatch(actual);
        }
    }
}

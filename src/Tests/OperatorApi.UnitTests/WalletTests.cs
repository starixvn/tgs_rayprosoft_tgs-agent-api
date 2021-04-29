using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Operator.Wallet;
using ExpectedObjects;
using Moq;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.Wallet;
using TGS.Core.Models.Services.Caches;
using System;
using TGS.Core.Models.Enums;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class WalletTests
    {
        [Test]
        public async Task Player_Can_Get_Balance_Info()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();
            var moqPlayerOperations = new Mock<IPlayerOperations>();

            var sut = new GetBalanceQueryService(moqWalletRepository.Object, moqPlayerOperations.Object);

            var playerCode = "ThisIsPlayerCode";
            moqPlayerOperations.Setup(fake => fake.GetCachedPlayerAsync(It.Is<string>(x => x == playerCode)))
                .Returns(Task.FromResult(new CachedPlayer
                {
                    PlayerCode = playerCode,
                    Username = "userName",
                    BetLimitId = 0,
                    BrandId = Guid.NewGuid(),
                    BrandCode = "BrandCode",
                    DefaultWalletCurrencyCode = "CNY",
                    DefaultWalletId = Guid.NewGuid(),
                    ExternalCurrencyCode = "CNY",
                    Status = PlayerStatus.Enabled,
                    Type = PlayerType.RealPlayer
                }));
            var expected = new BalanceInfo(10000M).ToExpectedObject();

            moqWalletRepository
                .Setup(fake => fake.GetBalanceAsync(It.IsAny<Guid>(), It.Is<string>(x => x == playerCode)))
                .Returns(ValueTask.FromResult(10000M));

            var actual = await sut.GetAsync(new GetBalanceQuery(playerCode));

            expected.ShouldMatch(actual);
        }
    }
}

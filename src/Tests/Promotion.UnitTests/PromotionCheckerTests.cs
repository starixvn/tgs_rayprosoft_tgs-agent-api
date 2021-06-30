using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion;

namespace Promotion.UnitTests
{
    [TestFixture]
    public class PromotionCheckerTests
    {
        [Test]
        public async Task FirstDeposit_Should_Check_Well()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakePlayerDataRepository = Substitute.For<IPlayerDataRepository>();

            var sut = new FirstDepositChecker(fakePlayerService, fakePlayerDataRepository);

            fakePlayerService
                .GetPlayerByPlayerCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    FirstDepositOn = DateTimeOffset.Now
                });

            var actual = await sut.CheckAsync("ThisIsPlayerCode");

            Assert.IsFalse(actual);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Services;

namespace Promotion.UnitTests
{
    [TestFixture]
    public class PromotionTests
    {
        [Test]
        public async Task Check_Player_Can_Join_Promotion_Should_Be_True()
        {
            var fakePromotionRepository = Substitute.For<IPromotionRepository>();
            var fakeServiceProvider = Substitute.For<IServiceProvider>();
            var checkerFactory = new PromotionCheckerFactory(
                new Dictionary<string, Type>
                {
                    ["TestPromotionType"] = typeof(IPromotionChecker)
                },
                fakeServiceProvider);
            var sut = new CheckCanJoinPromotionQueryService(fakePromotionRepository, checkerFactory);

            var query = new CheckCanJoinPromotionQuery(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                Guid.NewGuid());

            fakePromotionRepository
                .GetPromotionByIdAsync(query.PromotionId)
                .Returns(new TGS.Core.DataServices.Tgs.Models.Promotion
                {
                    BrandId = query.BrandId,
                    PromotionType = "TestPromotionType"
                });

            var fakeChecker = Substitute.For<IPromotionChecker>();

            fakeChecker
                .CheckAsync(Arg.Any<string>())
                .Returns(true);

            fakeServiceProvider
                .GetService(Arg.Any<Type>())
                .Returns(fakeChecker);

            var actual = await sut.GetAsync(query);

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task Player_FirstDeposit_Checker()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();

            var sut = new FirstDepositChecker(fakePlayerService);

            fakePlayerService
                .GetPlayerByPlayerCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    FirstDepositOn = null
                });

            var actual = await sut.CheckAsync("ThisIsPlayerCode");

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task Player_DailyFirstDeposit_Checker()
        {
            var fakePlayerDataService = Substitute.For<IPlayerDataRepository>();

            var sut = new DailyFirstDepositChecker(fakePlayerDataService);

            fakePlayerDataService
                .CheckPlayerIsDailyFirstDepositAsync(Arg.Any<string>())
                .Returns(true);

            var actual = await sut.CheckAsync("ThisIsPlayerCode");

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task Player_DailySecondDeposit_Checker()
        {
            var fakePlayerDataService = Substitute.For<IPlayerDataRepository>();

            var sut = new DailySecondDepositChecker(fakePlayerDataService);

            fakePlayerDataService
                .CheckPlayerIsDailyFirstDepositAsync(Arg.Any<string>())
                .Returns(false);

            var actual = await sut.CheckAsync("ThisIsPlayerCode");

            Assert.IsTrue(actual);
        }
    }
}

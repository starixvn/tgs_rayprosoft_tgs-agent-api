using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Services;

namespace Promotion.UnitTests
{
    [TestFixture]
    public class PromotionTests
    {
        [Test]
        public async Task Can_Revert_Promotion()
        {
            var fakePromotionRepository = Substitute.For<IPromotionRepository>();

            var sut = new RevertPromotionAmountCommandService(
                fakePromotionRepository,
                NullLogger<RevertPromotionAmountCommandService>.Instance);

            var command = new RevertPromotionAmountCommand(Guid.NewGuid(), 1000M);

            var percentage = 10M;
            var bonusAmountLimit = 500M;
            fakePromotionRepository.GetPromotionByIdAsync(command.PromotionId).Returns(
                new TGS.Core.DataServices.Tgs.Models.Promotion
                {
                    Percentage = percentage,
                    BonusAmountLimit = bonusAmountLimit
                });

            var amount = command.Amount * percentage / 100M;
            await sut.ExecuteAsync(command);

            _ = fakePromotionRepository
                .Received(1)
                .RevertPromotionAmountAsync(Arg.Is(command.PromotionId), Arg.Is(amount));
        }

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

            var sut = new CheckCanJoinPromotionQueryService(
                fakePromotionRepository,
                checkerFactory);

            var query = new CheckCanJoinPromotionQuery(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                100);

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

            fakePromotionRepository
                .UpdatePromotionAmountAsync(query.PromotionId, Arg.Any<decimal>())
                .Returns(true);

            var actual = await sut.GetAsync(query);

            Assert.IsTrue(actual);
        }
    }
}

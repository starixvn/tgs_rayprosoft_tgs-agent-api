using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
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
        public async Task Player_DailyFirstDeposit_Rollback_Tag()
        {
            var fakePlayerDataService = Substitute.For<IPlayerDataRepository>();

            var sut = new DailyFirstDepositChecker(fakePlayerDataService);

            await sut.RollbackTagAsync("ThisIsPlayerCode");

            _ = fakePlayerDataService.Received(1)
                .RemovePlayerDailyDepositFlagAsync(Arg.Is("ThisIsPlayerCode"));
        }

        [Test]
        public async Task Player_DailyFirstDeposit_Tag()
        {
            var fakePlayerDataService = Substitute.For<IPlayerDataRepository>();

            var sut = new DailyFirstDepositChecker(fakePlayerDataService);

            await sut.TagPromotionTypeAsync("ThisIsPlayerCode");

            _ = fakePlayerDataService.Received(1)
                .SetPlayerDailyDepositFlagAsync(Arg.Is("ThisIsPlayerCode"));
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

        [Test]
        public async Task Player_FirstDeposit_Checker()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakePlayerDataRepository = Substitute.For<IPlayerDataRepository>();

            var sut = new FirstDepositChecker(fakePlayerService, fakePlayerDataRepository);

            fakePlayerService
                .GetPlayerByPlayerCodeAsync(Arg.Any<string>())
                .Returns(new Player { FirstDepositOn = null });

            var actual = await sut.CheckAsync("ThisIsPlayerCode");

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task Player_FirstDeposit_Commit_Tag()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakePlayerDataRepository = Substitute.For<IPlayerDataRepository>();

            var sut = new FirstDepositChecker(fakePlayerService, fakePlayerDataRepository);

            var playerCode = "ThisIsPlayerCode";
            var testTime = DateTimeOffset.Now;

            fakePlayerDataRepository.GetPlayerFirstDepositFlagTimeAsync(Arg.Is(playerCode))
                .Returns(testTime);

            await sut.CommitTagAsync(playerCode);

            _ = fakePlayerService.Received(1)
                .SetFirstDepositTimeAsync(Arg.Is(playerCode), Arg.Is(testTime));

            _ = fakePlayerDataRepository.Received(1)
                .RemovePlayerFirstDepositFlagAsync(Arg.Is(playerCode));
        }

        [Test]
        public async Task Player_FirstDeposit_Rollback_Tag()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakePlayerDataRepository = Substitute.For<IPlayerDataRepository>();

            var sut = new FirstDepositChecker(fakePlayerService, fakePlayerDataRepository);

            await sut.RollbackTagAsync("ThisIsPlayerCode");

            _ = fakePlayerDataRepository.Received(1)
                .RemovePlayerFirstDepositFlagAsync(Arg.Is("ThisIsPlayerCode"));
        }

        [Test]
        public async Task Player_FirstDeposit_Tag()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakePlayerDataRepository = Substitute.For<IPlayerDataRepository>();

            var sut = new FirstDepositChecker(fakePlayerService, fakePlayerDataRepository);

            await sut.TagPromotionTypeAsync("ThisIsPlayerCode");

            _ = fakePlayerDataRepository.Received(1)
                .SetPlayerFirstDepositFlagAsync(Arg.Is("ThisIsPlayerCode"));
        }

        [Test]
        public async Task Tag_Player_Deposit()
        {
            var fakeServiceProvider = Substitute.For<IServiceProvider>();
            var fakePromotionChecker = Substitute.For<IPromotionChecker>();

            fakeServiceProvider.GetService(Arg.Any<Type>())
                .Returns(fakePromotionChecker);

            var fakeCheckFactory = new PromotionCheckerFactory(
                new Dictionary<string, Type>
                {
                    ["FirstDeposit"] = typeof(object),
                    ["DailyFirstDeposit"] = typeof(object)
                },
                fakeServiceProvider);

            var sut = new TagPlayerDepositCommandService(fakeCheckFactory);

            var command = new TagPlayerDepositCommand("ThisIsPlayerCode");

            await sut.ExecuteAsync(command);

            _ = fakePromotionChecker.Received(2)
                .TagPromotionTypeAsync(Arg.Any<string>());
        }
    }
}

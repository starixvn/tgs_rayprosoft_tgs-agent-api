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

        [Test]
        public async Task FirstDeposit_Player_FirstDepositOn_Is_Null_Should_Return_True()
        {
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakePlayerDataRepository = Substitute.For<IPlayerDataRepository>();

            var sut = new FirstDepositChecker(fakePlayerService, fakePlayerDataRepository);

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

            await sut.CommitTagAsync(playerCode);

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

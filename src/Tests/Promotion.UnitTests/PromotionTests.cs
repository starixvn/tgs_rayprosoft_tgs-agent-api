using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using MockQueryable.Moq;
using Moq;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Handlers;
using TGS.Services.Promotion.Models.Promotion;
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
                new Dictionary<string, Type> {["TestPromotionType"] = typeof(IPromotionChecker)},
                fakeServiceProvider);
            var sut = new CheckCanJoinPromotionQueryService(fakePromotionRepository, checkerFactory);

            var query = new CheckCanJoinPromotionQuery(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                Guid.NewGuid(), 100);

            fakePromotionRepository
                .GetPromotionByIdAsync(query.PromotionId)
                .Returns(new TGS.Core.DataServices.Tgs.Models.Promotion
                {
                    BrandId = query.BrandId, PromotionType = "TestPromotionType"
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

            var sut = new FirstDepositChecker(fakePlayerService);

            fakePlayerService
                .GetPlayerByPlayerCodeAsync(Arg.Any<string>())
                .Returns(new Player {FirstDepositOn = null});

            var actual = await sut.CheckAsync("ThisIsPlayerCode");

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task Tag_Player_Deposit()
        {
            var fakePlayerDataService = Substitute.For<IPlayerDataRepository>();

            var sut = new TagPlayerDepositCommandService(fakePlayerDataService);

            var command = new TagPlayerDepositCommand("ThisIsPlayerCode");

            await sut.ExecuteAsync(command);

            _ = fakePlayerDataService
                .Received(1)
                .SetPlayerDepositAsync(command.PlayerCode);
        }

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
                    Percentage = percentage, BonusAmountLimit = bonusAmountLimit
                });

            var amount = command.Amount * percentage / 100M;
            await sut.ExecuteAsync(command);

            _ = fakePromotionRepository
                .Received(1)
                .RevertPromotionAmountAsync(Arg.Is(command.PromotionId), Arg.Is(amount));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Promotions(bool showAll)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var promotionServiceMock = new Mock<IPromotionRepository>();

            var promotionsQueryHandlerMock =
                new GetPromotionsQueryHandler(promotionServiceMock.Object, mockHttpContextAccessor.Object);

            var request = new GetPromotionsRequest
            {
                BrandId = Guid.NewGuid(),
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreatePromotionFixture()
                .CreateMany<TGS.Core.DataServices.Tgs.Models.Promotion>(20)
                .AsQueryable()
                .BuildMock().Object;

            foreach (var promotion in list)
            {
                promotion.BrandId = request.BrandId;
            }

            promotionServiceMock.Setup(x =>
                    x.GetPromotionQueries(
                        It.IsAny<Func<IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>,
                            IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>>>(), false))
                .Returns(list);

            var result = await promotionsQueryHandlerMock.Handle(request, new());
            promotionServiceMock.Verify(x => x.GetPromotionQueries(
                It.IsAny<Func<IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>,
                    IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>>>(), false));

            Assert.AreEqual(result.Items.Length, showAll ? 20 : 10);
        }

        private Fixture CreatePromotionFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<TGS.Core.DataServices.Tgs.Models.Promotion>(u => u
                .With(s => s.PromotionGameProviders, new PromotionGameProvider[] { })
                .With(s => s.PromotionVipLevels, new PromotionVipLevel[] { })
                .With(s => s.PromotionParticipants, new PromotionParticipant[] { })
                .With(s => s.Brand, new Brand())
                .With(s => s.Localization, "{}"));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}
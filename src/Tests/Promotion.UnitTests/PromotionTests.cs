using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Models.Promotion;
using TGS.Services.Promotion.Services;
using TGS.ServiceUtility;

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
        public async Task Get_Promotions_By_Player_Should_Return_Match_Promotions()
        {
            var fakeValidPromotionQueryService = Substitute.For<IQueryService<GetValidPromotionsQuery, PromotionModel>>();
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakePromotionCheckerFactory = Substitute.For<IPromotionCheckerFactory>();
            var fakePromotionChecker = Substitute.For<IPromotionChecker>();

            var sut = new GetValidPromotionsByPlayerQueryService(
                fakeValidPromotionQueryService,
                fakePlayerService,
                fakePromotionCheckerFactory);

            fakePromotionCheckerFactory.GetPromotionChecker(Arg.Any<string>())
                .Returns(fakePromotionChecker);

            fakePromotionChecker.CheckAsync(Arg.Any<string>())
                .Returns(true);

            var testData = new[] {
                new PromotionModel
                {
                    Id = Guid.NewGuid(),
                    VipLevels = Array.Empty<int>()
                },
                new PromotionModel
                {
                    Id = Guid.NewGuid(),
                    VipLevels = Array.Empty<int>()
                },
                new PromotionModel
                {
                    Id = Guid.NewGuid(),
                    VipLevels = Array.Empty<int>()
                }
            };

            var expected = testData.ToExpectedObject();

            var query = new GetValidPromotionsByPlayerQuery(Guid.NewGuid(), "ThisIsPlayerCode");

            fakePlayerService
                .GetPlayerAsync(
                    Arg.Any<Expression<Func<Player, bool>>>(),
                    Arg.Any<Func<IQueryable<Player>, IQueryable<Player>>>())
                .Returns(new Player
                {
                    PlayerCode = query.PlayerCode,
                    Vip = new VipLevel
                    {
                        Level = 1
                    }
                });

            fakeValidPromotionQueryService.QueryAsync(Arg.Any<GetValidPromotionsQuery>())
                .Returns(testData.ToAsyncEnumerable());

            var actual = await sut.QueryAsync(query).ToArrayAsync();

            expected.ShouldMatch(actual);
        }

        [Test]
        public void Get_Promotions_Without_Player_Should_Return_All_Match_Promotions()
        {
            var fakePromotionRepository = Substitute.For<IPromotionRepository>();

            var sut = new GetValidPromotionsQueryService(fakePromotionRepository);

            var query = new GetValidPromotionsQuery(Guid.NewGuid());

            sut.QueryAsync(query);

            fakePromotionRepository.Received(1)
                .GetPromotionQueries(
                    Arg.Any<Func<
                       IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>, 
                       IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>>>(),
                    Arg.Any<bool>());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;
using TGS.Services.Report.Application.Models.Round;
using TGS.Services.Report.Application.Queries.Round;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Tests.ReportApi.UnitTests
{
    [TestFixture]
    public class PlayerReportTests
    {
        [Test]
        public async Task Player_Report_Should_Order_Desc_By_CreateOn()
        {
            // arrange
            var playerCode = "playerCode";
            var brandId = Guid.NewGuid();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var gameProviderId = new Guid("de301075-182b-453f-a316-28e3164b2d06");
            var gameId = 400029;
            var username = "username";
            var gameType = GameType.Slot;
            var gpCode = "HABA";

            var roundList = new List<Round>();
            var round1 = new Round()
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                CreatedOn = new DateTimeOffset(new DateTime(2021, 4, 20)),
                GameProviderId = gameProviderId,
                ClientStatus = ClientRoundStatus.Win,
                PlayerCode = playerCode,
                GameId = gameId
            };
            roundList.Add(round1);
            var round2 = new Round()
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                CreatedOn = new DateTimeOffset(new DateTime(2021, 4, 22)),
                GameProviderId = gameProviderId,
                ClientStatus = ClientRoundStatus.Loss,
                PlayerCode = playerCode,
                GameId = gameId
            };
            roundList.Add(round2);
            var round3 = new Round()
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                CreatedOn = new DateTimeOffset(new DateTime(2021, 5, 9)),
                GameProviderId = gameProviderId,
                ClientStatus = ClientRoundStatus.Draw,
                PlayerCode = playerCode,
                GameId = gameId
            };
            roundList.Add(round3);

            var round4 = new Round()
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                CreatedOn = new DateTimeOffset(new DateTime(2021, 5, 10)),
                GameProviderId = gameProviderId,
                ClientStatus = ClientRoundStatus.Loss,
                PlayerCode = playerCode,
                GameId = gameId
            };
            roundList.Add(round4);
            var round5 = new Round()
            {
                Id = Guid.NewGuid(),
                BrandId = brandId,
                CreatedOn = new DateTimeOffset(new DateTime(2021, 5, 15)),
                GameProviderId = gameProviderId,
                ClientStatus = ClientRoundStatus.Loss,
                PlayerCode = playerCode,
                GameId = gameId
            };
            roundList.Add(round5);

            await tgsRepository.Rounds.AddRangeAsync(roundList);

            await tgsRepository.Players.AddAsync(
                new Player()
                {
                    PlayerCode = playerCode,
                    BrandId = brandId,
                    Username = username
                });

            await tgsRepository.Games.AddAsync(
                new Game()
                {
                    Id = gameId,
                    Type = gameType,
                    GameProvider = new GameProvider() { Code = gpCode }
                });

            await tgsRepository.RoundDetails.AddRangeAsync(
                new RoundDetail()
                {
                    Round = round1,
                    RoundId = Guid.NewGuid()
                },
                new RoundDetail()
                {
                    Round = round2,
                    RoundId = Guid.NewGuid()
                },
                new RoundDetail()
                {
                    Round = round3,
                    RoundId = Guid.NewGuid()
                },
                new RoundDetail()
                {
                    Round = round4,
                    RoundId = Guid.NewGuid()
                },
                new RoundDetail()
                {
                    Round = round5,
                    RoundId = Guid.NewGuid()
                }
                );

            await tgsRepository.GameProviders.AddAsync(new GameProvider() { Id = gameProviderId });

            await tgsRepository.SaveChangesAsync();

            var tgsRoundService = new TgsRoundService(tgsRepository);

            var fakeHttpContext = new DefaultHttpContext();
            fakeHttpContext.Items.Add(Consts.ContextItems.BrandId, brandId);
            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData
            {
                PlayerCode = playerCode
            });

            var fakeHttpContextAccessor = new HttpContextAccessor { HttpContext = fakeHttpContext };

            var sut = new GetRoundsQueryHandler(tgsRoundService, fakeHttpContextAccessor);

            var request = new GetRoundsRequest()
            {
                From = new DateTimeOffset(new DateTime(2021, 4, 19)),
                To = new DateTimeOffset(new DateTime(2021, 5, 19, 23, 59, 59)),
                Page = 2,
                Rows = 2,
                PlayerCode = playerCode,
                BrandId = brandId,
                GameProviderId = gameProviderId,
                Username = username,
                GameType = gameType
            };

            var excepted = new DateTimeOffset(new DateTime(2021, 5, 9));

            // act
            var r = await sut.Handle(request, default);

            // assert
            var actual = r.Items.FirstOrDefault().CreatedOn;
            Assert.AreEqual(excepted, actual);
        }
    }
}
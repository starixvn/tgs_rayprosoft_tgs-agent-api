using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Controllers;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Tokens;
using TGS.Operator.Repositories;
using TGS.Paging;
using TGS.Services.Operator;
using TGS.Services.Operator.StationLetters;
using TGS.ServiceUtility;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class StationLetterTests
    {
        [Test]
        public void Player_Can_List_Station_Letters()
        {
            var moqStationLetterRepository = new Mock<IStationLetterRepository>();

            var sut = new GetStationLettersQueryService(moqStationLetterRepository.Object);

            var playerCode = "ThisIsPlayerCode";

            var actual = sut.QueryAsync(new GetStationLettersQuery(playerCode, new PageableRequest()));

            moqStationLetterRepository.Verify(
                fake => fake.QueryStationLettersAsync(It.Is<string>(x => x == playerCode), It.IsAny<Paging<Services.Operator.StationLetter>>()),
                Times.Exactly(1));
        }

        [Test]
        public void Player_Can_Set_Station_Letters_HaveRead()
        {
            var moqStationLetterRepository = new Mock<IStationLetterRepository>();

            var sut = new SetHaveReadCommandService(moqStationLetterRepository.Object);

            var playerCode = "ThisIsPlayerCode";
            var letterId = Guid.NewGuid();

            sut.ExecuteAsync(new SetHaveReadCommand(playerCode, letterId));

            moqStationLetterRepository.Verify(
                fake => fake.SetStationLetterHaveReadAsync(It.Is<string>(x => x == playerCode), It.Is<Guid>(x => x == letterId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Set_Db_Station_Letter_HaveRead_Should_Commit()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);
            var sut = new StationLetterRepository(tgsRepository);

            var playerCode = "ThisIsPlayerCode";
            var letterId = Guid.NewGuid();

            tgsRepository.StationLetters.Add(new Core.DataServices.Tgs.Models.StationLetter
            {
                Id = letterId,
                BrandId = Guid.NewGuid(),
                Subject = "test",
                Content = "content"
            });

            var targetPlayerLetter = new StationLetterPlayer
            {
                StationLetterId = letterId,
                PlayerCode = playerCode,
                IsRead = false
            };

            tgsRepository.StationLetterPlayers.Add(targetPlayerLetter);

            await tgsRepository.SaveChangesAsync();

            await sut.SetStationLetterHaveReadAsync(playerCode, letterId);

            Assert.IsTrue(targetPlayerLetter.IsRead);
            Assert.IsNotNull(targetPlayerLetter.ReadOn);
        }

        [Test]
        public void Player_Can_Get_Station_Letters_Count()
        {
            var moqStationLetterRepository = new Mock<IStationLetterRepository>();

            var sut = new CountStationLettersQueryService(moqStationLetterRepository.Object);

            var playerCode = "ThisIsPlayerCode";

            var actual = sut.GetAsync(new GetStationLettersQuery(playerCode, new PageableRequest()));

            moqStationLetterRepository.Verify(
                fake => fake.QueryStationLettersTotalCountAsync(It.Is<string>(x => x == playerCode)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Player_Can_Get_Station_Letters_With_Status_Published()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var playerCode = "playerCode";

            List<StationLetterPlayer> stationLetterPlayers = new List<StationLetterPlayer>();
            stationLetterPlayers.Add(new StationLetterPlayer()
            {
                PlayerCode = playerCode,
                StationLetter = new Core.DataServices.Tgs.Models.StationLetter()
                {
                    Status = StationLetterStatus.InVerify
                }
            });
            stationLetterPlayers.Add(new StationLetterPlayer()
            {
                PlayerCode = playerCode,
                StationLetter = new Core.DataServices.Tgs.Models.StationLetter()
                {
                    Status = StationLetterStatus.Published
                }
            });
            stationLetterPlayers.Add(new StationLetterPlayer()
            {
                PlayerCode = playerCode,
                StationLetter = new Core.DataServices.Tgs.Models.StationLetter()
                {
                    Status = StationLetterStatus.Rejected
                }
            });

            await repository.StationLetterPlayers.AddRangeAsync(stationLetterPlayers);
            await repository.SaveChangesAsync();

            var sut = new StationLetterRepository(repository);

            var actual = await sut.QueryStationLettersAsync(playerCode, new Paging<Services.Operator.StationLetter>() { Page = 0, RowCount = 50 }).ToListAsync();

            Assert.AreEqual(1, actual.Count);
        }

        [Test]
        public async Task Player_Station_Letters_Should_Order_Desc_By_CreateOn()
        {
            // arrange
            var fakeGetStationLetterService = Substitute.For<IQueryService<GetStationLettersQuery, Services.Operator.StationLetter>>();
            var fakeCountStationLetterService = Substitute.For<IGetService<GetStationLettersQuery, int>>();
            var request = new PagingRequest()
            {
                Page = 1,
                Rows = 10,
                IsDesc = true,
                SortBy = string.Empty,
                ShowAll = false
            };

            var playerCode = "playerCode";
            var fakeHttpContext = new DefaultHttpContext();
            var brandId = Guid.NewGuid();
            fakeHttpContext.Items.Add(Consts.ContextItems.BrandId, brandId);
            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData
            {
                PlayerCode = playerCode
            });
            var subject = "Test subject";
            var content = "Test content";

            var stationLetters = new List<Services.Operator.StationLetter>
            {
                new Services.Operator.StationLetter(
                    Guid.NewGuid(),
                    subject,
                    content,
                    CreateOn: new DateTimeOffset(new DateTime(2021,4,19)),
                    false)
                ,
                new Services.Operator.StationLetter(
                    Guid.NewGuid(),
                    subject,
                    content,
                    CreateOn: new DateTimeOffset(new DateTime(2021,5,10)),
                    false)
                ,
                new Services.Operator.StationLetter(
                    Guid.NewGuid(),
                    subject,
                    content,
                    CreateOn: new DateTimeOffset(new DateTime(2021,5,19)),
                    false)
            };

            var paging = new PageableRequest();
            if (request.ShowAll)
                paging.ShowAll = true;
            else
            {
                paging.Page = request.Page;
                paging.Rows = request.Rows;
                paging.OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending;
                paging.SortBy = request.SortBy;
            }

            fakeGetStationLetterService
                .QueryAsync(Arg.Any<GetStationLettersQuery>())
                .Returns(stationLetters.ToAsyncEnumerable());

            var sut = new StationLetterController();
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = fakeHttpContext
            };

            // act
            var r = await sut.GetLettersAsync(fakeGetStationLetterService, fakeCountStationLetterService, request);

            // assert
            Assert.That(r.Items, Is.Ordered.Descending.By("CreateOn"));
        }
    }
}
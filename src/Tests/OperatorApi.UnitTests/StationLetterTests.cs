using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using TGS.Operator.Repositories;
using TGS.Paging;
using TGS.Services.Operator;
using TGS.Services.Operator.StationLetters;

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
                    Status = Core.Models.Enums.StationLetterStatus.InVerify
                }
            });
            stationLetterPlayers.Add(new StationLetterPlayer()
            {
                PlayerCode = playerCode,
                StationLetter = new Core.DataServices.Tgs.Models.StationLetter()
                {
                    Status = Core.Models.Enums.StationLetterStatus.Published
                }
            });
            stationLetterPlayers.Add(new StationLetterPlayer()
            {
                PlayerCode = playerCode,
                StationLetter = new Core.DataServices.Tgs.Models.StationLetter()
                {
                    Status = Core.Models.Enums.StationLetterStatus.Rejected
                }
            });

            await repository.StationLetterPlayers.AddRangeAsync(stationLetterPlayers);
            await repository.SaveChangesAsync();

            var sut = new StationLetterRepository(repository);

            var actual = await sut.QueryStationLettersAsync(playerCode, new Paging<Services.Operator.StationLetter>() { Page = 0, RowCount = 50 }).ToListAsync();

            Assert.AreEqual(1, actual.Count);
        }
    }
}
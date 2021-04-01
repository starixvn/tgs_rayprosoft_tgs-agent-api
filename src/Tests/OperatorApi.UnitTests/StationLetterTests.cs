using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using TGS.Operator.Repositories;
using TGS.Services.Operator;
using TGS.Services.Operator.StationLatters;
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
        public async System.Threading.Tasks.Task Set_Db_Station_Letter_HaveRead_Should_Commit()
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
    }
}

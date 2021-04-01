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

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class StationLatterTests
    {
        [Test]
        public void Player_Can_List_Station_Latters()
        {
            var moqStationLatterRepository = new Mock<IStationLatterRepository>();

            var sut = new ListStationLattersQueryService(moqStationLatterRepository.Object);

            var playerCode = "ThisIsPlayerCode";

            var actual = sut.QueryAsync(new ListStationLattersQuery(playerCode, new PageableRequest()));

            moqStationLatterRepository.Verify(
                fake => fake.QueryStationLattersAsync(It.Is<string>(x => x == playerCode), It.IsAny<Paging<StationLatter>>()),
                Times.Exactly(1));
        }

        [Test]
        public void Player_Can_Set_Station_Latters_HaveRead()
        {
            var moqStationLatterRepository = new Mock<IStationLatterRepository>();

            var sut = new SetHaveReadCommandService(moqStationLatterRepository.Object);

            var playerCode = "ThisIsPlayerCode";
            var latterId = Guid.NewGuid();

            sut.ExecuteAsync(new SetHaveReadCommand(playerCode, latterId));

            moqStationLatterRepository.Verify(
                fake => fake.SetStationLatterHaveReadAsync(It.Is<string>(x => x == playerCode), It.Is<Guid>(x => x == latterId)),
                Times.Exactly(1));
        }

        [Test]
        public async System.Threading.Tasks.Task Set_Db_Station_Latter_HaveRead_Should_Commit()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);
            var sut = new StationLatterRepository(tgsRepository);

            var playerCode = "ThisIsPlayerCode";
            var latterId = Guid.NewGuid();

            tgsRepository.StationLetters.Add(new StationLetter
            {
                Id = latterId,
                BrandId = Guid.NewGuid(),
                Subject = "test",
                Content = "content"
            });

            var targetPlayerLatter = new StationLetterPlayer
            {
                StationLetterId = latterId,
                PlayerCode = playerCode,
                IsRead = false
            };

            tgsRepository.StationLetterPlayers.Add(targetPlayerLatter);

            await tgsRepository.SaveChangesAsync();

            await sut.SetStationLatterHaveReadAsync(playerCode, latterId);

            Assert.IsTrue(targetPlayerLatter.IsRead);
            Assert.IsNotNull(targetPlayerLatter.ReadOn);
        }
    }
}

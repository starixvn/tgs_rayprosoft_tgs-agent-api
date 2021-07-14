using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Promotion;

namespace Promotion.UnitTests
{
    [TestFixture]
    public class PlayerServiceTests
    {
        [Test]
        public async Task Set_Player_FirstDepositOn_If_Source_Null_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var sut = new PlayerService(tgsRepository);

            var playerCode = "ThisIsPlayerCode";
            var depositOn = DateTimeOffset.Now;

            await tgsRepository.Players
                .AddAsync(new TGS.Core.DataServices.Tgs.Models.Player
                {
                    PlayerCode = playerCode
                });

            await tgsRepository.SaveChangesAsync();

            await sut.SetFirstDepositTimeAsync(playerCode, depositOn);

            var actual = await tgsRepository.Players.FindAsync(playerCode);

            Assert.AreEqual(depositOn, actual.FirstDepositOn);
        }

        [Test]
        public async Task Set_Player_FirstDepositOn_If_Source_Set_Should_Ignore()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var sut = new PlayerService(tgsRepository);

            var playerCode = "ThisIsPlayerCode";
            var depositOn = DateTimeOffset.Now;

            await tgsRepository.Players
                .AddAsync(new TGS.Core.DataServices.Tgs.Models.Player
                {
                    PlayerCode = playerCode,
                    FirstDepositOn = DateTimeOffset.Parse("2021/1/1")
                });

            await tgsRepository.SaveChangesAsync();

            await sut.SetFirstDepositTimeAsync(playerCode, depositOn);

            var actual = await tgsRepository.Players.FindAsync(playerCode);

            Assert.NotNull(actual.FirstDepositOn);
            Assert.AreNotEqual(depositOn, actual.FirstDepositOn);
        }
    }
}

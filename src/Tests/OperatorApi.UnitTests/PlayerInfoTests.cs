using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Player;
using CoreEnums = TGS.Core.Models.Enums;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerInfoTests
    {
        [Test]
        public async Task Set_Player_QQ_Number_Should_Success()
        {
            var moqPlayerOperations = new Mock<IPlayerOperations>();

            var sut = new SetQqCommandService(moqPlayerOperations.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "TBH"
            };

            var command = new SetQqCommand(player.PlayerCode, "this_is_qq_number");

            moqPlayerOperations
                .Setup(fake => fake.GetPlayerByCodeAsync(
                    It.Is<string>(x => x == player.PlayerCode)))
                .Returns(Task.FromResult(player));

            await sut.ExecuteAsync(command);

            moqPlayerOperations
                .Verify(
                    fake => fake.SetQqAsync(
                        It.Is<string>(x => x == player.PlayerCode),
                        It.Is<string>(x => x == command.QqId),
                        It.IsAny<DateTimeOffset>()),
                    Times.Exactly(1));
        }

        [Test]
        public async Task Set_Player_WeChat_Number_Should_Success()
        {
            var moqPlayerOperations = new Mock<IPlayerOperations>();

            var sut = new SetWeChatCommandService(moqPlayerOperations.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "TBH"
            };

            var command = new SetWeChatCommand(player.PlayerCode, "this_is_wechat_number");

            moqPlayerOperations
                .Setup(fake => fake.GetPlayerByCodeAsync(
                    It.Is<string>(x => x == player.PlayerCode)))
                .Returns(Task.FromResult(player));

            await sut.ExecuteAsync(command);

            moqPlayerOperations
                .Verify(
                    fake => fake.SetWeChatAsync(
                        It.Is<string>(x => x == player.PlayerCode),
                        It.Is<string>(x => x == command.WeChatId),
                        It.IsAny<DateTimeOffset>()),
                    Times.Exactly(1));
        }

        [Test]
        public async Task Get_Player_Status_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);
            var brandId = Guid.NewGuid();
            var playerCode = "playerCode";

            repository.Players.Add(new Player
            {
                PlayerCode = playerCode,
                Status = CoreEnums.PlayerStatus.Disabled
            });

            await repository.SaveChangesAsync();
            var sut = new GetPlayerStatusQueryService(repository);

            var query = new GetPlayerStatusQuery(playerCode);
            var actual = await sut.GetAsync(query);
            var expected = CoreEnums.PlayerStatus.Disabled;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Check_Player_Status_InBlacklist_Should_Throw_Exception()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var player = new Player { Status = CoreEnums.PlayerStatus.InBlacklist };

            fakePlayerOperations
                .GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(player);
            var sut = new CheckPlayerBannedCommandService(fakePlayerOperations);

            var command = new CheckPlayerBannedCommand(
                Guid.NewGuid(),
                "UserName");
            Assert.ThrowsAsync<UserBannedException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Query_Player_By_UserName_Should_Success()
        {
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();

            var sut = new GetPlayerInfoByUserNameQueryService(fakePlayerRepository);

            var playerInfo = new PlayerInfo
            ("playerCode",
                100,
                false,
                false,
                Guid.NewGuid()
            );

            fakePlayerRepository.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(ValueTask.FromResult(playerInfo));

            var brandId = Guid.NewGuid();
            var username = "username";
            var query = new GetPlayerInfoByUserNameQuery(brandId, username);

            var actual = await sut.GetAsync(query);

            Assert.AreEqual(playerInfo, actual);
        }
    }
}
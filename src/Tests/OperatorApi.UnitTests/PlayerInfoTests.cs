using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operations;
using TGS.Services.Operator.PlayerInfo;

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
    }
}

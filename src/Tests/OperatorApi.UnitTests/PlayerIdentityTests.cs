using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operations;
using TGS.Services.Operator.Player;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerIdentityTests
    {
        [Test]
        public async Task Set_Player_Identity_Should_Success()
        {
            var moqPlayerOperations = new Mock<IPlayerOperations>();

            var sut = new SetIdentityCommandService(moqPlayerOperations.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "TBH"
            };
            var command = new SetIdentityCommand(player.PlayerCode, "RealName", "123456789");

            moqPlayerOperations
                .Setup(fake => fake.GetPlayerByCodeAsync(
                    It.Is<string>(x => x == player.PlayerCode)))
                .Returns(Task.FromResult(player));

            await sut.ExecuteAsync(command);

            moqPlayerOperations.Verify(
                fake => fake.SetIdentityAsync(
                    It.Is<string>(x => x == player.PlayerCode),
                    It.Is<string>(x => x == command.RealName),
                    It.Is<string>(x => x == command.Identity),
                    It.Is<bool>(x => x == false),
                    It.IsAny<DateTimeOffset>()),
                Times.Exactly(1));
        }
    }
}

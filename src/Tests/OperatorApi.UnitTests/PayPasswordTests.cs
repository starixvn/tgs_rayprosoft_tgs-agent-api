using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.PayPassowrd;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PayPasswordTests
    {
        [Test]
        public async Task Set_Pay_Password_Should_Success()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var sut = new SetFirstPayPasswordCommandService(
                fakePlayerOperations,
                fakePasswordHandler);

            var playerCode = "testPlayerCode";
            var password = "123456";
            var command = new SetFirstPayPasswordCommand(playerCode, password);

            fakePlayerOperations
                .GetPlayerByCodeAsync(Arg.Is(playerCode))
                .Returns(new Player
                {
                    PlayerCode = playerCode,
                    Username = "Tester",
                    WithdrawPassword = Array.Empty<byte>()
                });

            await sut.ExecuteAsync(command);

            fakePasswordHandler.Received(1).GenerateCredential(Arg.Is(password));
            _ = fakePlayerOperations
                .Received(1)
                .SetPayPasswordAsync(Arg.Is(playerCode), Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public void If_Already_Set_Pay_Password_Will_Throw_Exception()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var sut = new SetFirstPayPasswordCommandService(
                fakePlayerOperations,
                fakePasswordHandler);

            var playerCode = "testPlayerCode";
            var password = "123456";
            var command = new SetFirstPayPasswordCommand(playerCode, password);

            fakePlayerOperations
                .GetPlayerByCodeAsync(Arg.Is(playerCode))
                .Returns(new Player
                {
                    PlayerCode = playerCode,
                    Username = "Tester",
                    WithdrawPassword = new byte[] { 1, 2, 3 }
                });

            Assert.ThrowsAsync<PayPasswordAlreadySetException>(async () => await sut.ExecuteAsync(command));
        }
    }
}

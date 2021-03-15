using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

        [Test]
        public async Task Start_Change_Pay_Password_Should_Send_Short_Message()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new StartChangePayPasswordCommandService(
                fakePlayerOperations,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                Phone = "0988123456",
                PhoneAuthentication = true,
                DefaultWalletCurrencyCode = "CNY"
            };

            var command = new StartChangePayPasswordCommand(player.PlayerCode, CultureInfo.GetCultureInfo("en-US"));

            var validateCode = "123456";
            var token = "abc123";

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Is(player.PlayerCode)).Returns(player);

            fakeValidateCodeGenerator.GenerateCode().Returns(validateCode);
            fakeValidateCodeGenerator.GenerateToken().Returns(token);

            fakeValidateCodeOperator
                .SaveValidateDataAsync<ValidateCodeData>(Arg.Is(validateCode), Arg.Is(token), Arg.Any<ValidateCodeData>())
                .Returns(true);

            var expected = $"Your ID verification code is : {validateCode}";

            await sut.GetAsync(command);

            _ = fakeValidateCodeOperator
                .Received(1)
                .SendMessageAsync(
                    Arg.Is(player.Phone),
                    Arg.Is(expected),
                    Arg.Is(player.DefaultWalletCurrencyCode),
                    Arg.Any<Guid>());
        }

        [Test]
        public async Task Change_Pay_Password_Should_Success()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new ChangePayPasswordCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeValidateCodeOperator);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                Phone = "0988123456",
                PhoneAuthentication = true,
                DefaultWalletCurrencyCode = "CNY"
            };

            var newPassword = "newPassword";
            var validateCode = "123456";
            var token = "99CFACE4-03FE-42A2-B1DB-57C93A7BCD2E";

            var command = new ChangePayPasswordCommand(
                player.PlayerCode,
                newPassword,
                validateCode,
                token);

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Is(player.PlayerCode)).Returns(player);
            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<ValidateCodeData>(Arg.Is(validateCode), Arg.Is(token))
                .Returns(new ValidateCodeData(player.PlayerCode, token));

            await sut.ExecuteAsync(command);

            fakePasswordHandler.Received(1).GenerateCredential(Arg.Is(newPassword));
            _ = fakePlayerOperations
                .Received(1)
                .SetPayPasswordAsync(Arg.Is(player.PlayerCode), Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public void Change_Pay_Password_If_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new ChangePayPasswordCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeValidateCodeOperator);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                Phone = "0988123456",
                PhoneAuthentication = true,
                DefaultWalletCurrencyCode = "CNY"
            };

            var newPassword = "newPassword";
            var validateCode = "123456";
            var token = "99CFACE4-03FE-42A2-B1DB-57C93A7BCD2E";

            var command = new ChangePayPasswordCommand(
                player.PlayerCode,
                newPassword,
                validateCode,
                token);

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Is(player.PlayerCode)).Returns(player);
            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<ValidateCodeData>(Arg.Is(validateCode), Arg.Is(token))
                .Returns((ValidateCodeData)null);

            Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }
    }
}

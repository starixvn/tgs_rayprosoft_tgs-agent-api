using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Apis.AgentApi.Validators;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.PayPassowrd;
using TGS.Services.Agent.ValidateCode;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PayPasswordTests
    {
        [Test]
        public void Change_Pay_Password_If_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new ChangePayPasswordCommandService(
                fakeAgentRepository,
                fakePasswordHandler,
                fakeValidateCodeOperator);

            var agent = new AgentInfo(
                Guid.NewGuid(),
                "TestAgent",
                Guid.NewGuid(),
                "test@gmail.com",
                null,
                null);

            var newPassword = "newPassword";
            var validateCode = "123456";
            var token = "99CFACE4-03FE-42A2-B1DB-57C93A7BCD2E";

            var command = new ChangePayPasswordCommand(
                agent.Id,
                newPassword,
                validateCode,
                token);

            fakeAgentRepository.GetAgentByIdAsync(Arg.Is(agent.Id)).Returns(agent);
            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<ValidateCodeData>(Arg.Is(validateCode), Arg.Is(token))
                .Returns((ValidateCodeData)null);

            Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Change_Pay_Password_Should_Success()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new ChangePayPasswordCommandService(
                fakeAgentRepository,
                fakePasswordHandler,
                fakeValidateCodeOperator);

            var agent = new AgentInfo(
                Guid.NewGuid(),
                "TestAgent",
                Guid.NewGuid(),
                "test@gmail.com",
                null,
                null);

            var newPassword = "newPassword";
            var validateCode = "123456";
            var token = "99CFACE4-03FE-42A2-B1DB-57C93A7BCD2E";

            var command = new ChangePayPasswordCommand(
                agent.Id,
                newPassword,
                validateCode,
                token);

            fakeAgentRepository.GetAgentByIdAsync(Arg.Is(agent.Id)).Returns(agent);
            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<ValidateCodeData>(Arg.Is(validateCode), Arg.Is(token))
                .Returns(new ValidateCodeData(agent.Id.ToString(), token));

            await sut.ExecuteAsync(command);

            fakePasswordHandler.Received(1).GenerateCredential(Arg.Is(newPassword));
            _ = fakeAgentRepository
                .Received(1)
                .SetPayPasswordAsync(Arg.Is(agent.Id), Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public void If_Already_Set_Pay_Password_Will_Throw_Exception()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var sut = new SetFirstPayPasswordCommandService(
                fakeAgentRepository,
                fakePasswordHandler);

            var agentId = Guid.NewGuid();
            var password = "123456";
            var command = new SetFirstPayPasswordCommand(agentId, password);

            fakeAgentRepository
                .GetAgentByIdAsync(Arg.Is(agentId))
                .Returns(new AgentInfo(
                    Guid.NewGuid(),
                    "TestAgent",
                    Guid.NewGuid(),
                    "test@gmail.com",
                    new byte[] { 1, 2, 3 },
                    null));

            Assert.ThrowsAsync<PayPasswordAlreadySetException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Set_Pay_Password_Should_Success()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var sut = new SetFirstPayPasswordCommandService(
                fakeAgentRepository,
                fakePasswordHandler);

            var agentId = Guid.NewGuid();
            var password = "123456";
            var command = new SetFirstPayPasswordCommand(agentId, password);

            fakeAgentRepository
                .GetAgentByIdAsync(Arg.Is(agentId))
                .Returns(new AgentInfo(
                    Guid.NewGuid(),
                    "TestAgent",
                    Guid.NewGuid(),
                    "test@gmail.com",
                    null,
                    null));

            await sut.ExecuteAsync(command);

            fakePasswordHandler.Received(1).GenerateCredential(Arg.Is(password));
            _ = fakeAgentRepository
                .Received(1)
                .SetPayPasswordAsync(Arg.Is(agentId), Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public async Task Start_Change_Pay_Password_Should_Send_Confirm_Email()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new StartChangePayPasswordCommandService(
                fakeAgentRepository,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var email = "email@test.com";
            var agent = new AgentInfo(
                Guid.NewGuid(),
                "TestAgent",
                Guid.NewGuid(),
                email,
                null,
                null);

            var command = new StartChangePayPasswordCommand(agent.Id, CultureInfo.GetCultureInfo("en-US"));

            var validateCode = "123456";
            var expectedSecurityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            fakeAgentRepository.GetAgentByIdAsync(Arg.Is(agent.Id)).Returns(agent);

            fakeValidateCodeGenerator.GenerateCode().Returns(validateCode);
            fakeValidateCodeGenerator.GenerateSecurityKey().Returns(expectedSecurityKey);

            fakeValidateCodeOperator
                .SaveValidateDataAsync(
                    Arg.Is(validateCode),
                    Arg.Is(expectedSecurityKey),
                    Arg.Any<ValidateCodeData>())
                .Returns(true);

            var actualSecurityKey = await sut.GetAsync(command);

            _ = fakeValidateCodeOperator
                .Received(1)
                .SendEmailAsync(
                    Arg.Is(command.CultureInfo),
                    Arg.Is(agent.Email),
                    Arg.Is(validateCode));

            Assert.AreEqual(expectedSecurityKey, actualSecurityKey);
        }

        [Test]
        public void Start_Change_Pay_Password_Should_Verify_Email()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var agent = new AgentInfo(
                Guid.NewGuid(),
                "TestAgent",
                Guid.NewGuid(),
                null,
                null,
                null);
            fakeAgentRepository.GetAgentByIdAsync(Arg.Is(agent.Id)).Returns(agent);
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var command = new StartChangePayPasswordCommand(agent.Id, CultureInfo.GetCultureInfo("en-US"));

            var sut = new StartChangePayPasswordCommandService(
                fakeAgentRepository,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            Assert.ThrowsAsync<EmailNotVerifyException>(async () => await sut.GetAsync(command));
        }

        [Test]
        public void Verify_Change_Pay_Password_Input_Max_Length()
        {
            // arrange
            var sut = new ChangePayPasswordRequestValidator();
            var request = new ChangePayPasswordRequest()
            {
                NewPassword = "123456789012345678901"
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.NewPassword);
        }

        [Test]
        public void Verify_Change_Pay_Password_Input_Min_Length()
        {
            // arrange
            var sut = new ChangePayPasswordRequestValidator();
            var request = new ChangePayPasswordRequest()
            {
                NewPassword = "12345"
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.NewPassword);
        }

        [Test]
        public void Verify_Change_Pay_Password_Input_With_Empty()
        {
            // arrange
            var sut = new ChangePayPasswordRequestValidator();
            var request = new ChangePayPasswordRequest()
            {
                NewPassword = string.Empty
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.NewPassword);
        }

        [Test]
        public void Verify_Set_Pay_Password_Input_Max_Length()
        {
            // arrange
            var sut = new SetPayPasswordRequestValidator();
            var request = new SetPayPasswordRequest()
            {
                Password = "123456789012345678901"
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }

        [Test]
        public void Verify_Set_Pay_Password_Input_Min_Length()
        {
            // arrange
            var sut = new SetPayPasswordRequestValidator();
            var request = new SetPayPasswordRequest()
            {
                Password = "12345"
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }

        [Test]
        public void Verify_Set_Pay_Password_Input_With_Empty()
        {
            // arrange
            var sut = new SetPayPasswordRequestValidator();
            var request = new SetPayPasswordRequest()
            {
                Password = string.Empty
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }
    }
}

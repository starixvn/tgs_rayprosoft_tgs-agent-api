using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Apis.AgentApi.Validators;
using TGS.Core.Models.Enums;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Agent;
using TGS.Services.Agent.ValidateCode;
using TGS.SmsService.Client;

namespace Agent.UnitTests
{
    public class AgentPasswordTests
    {
        [Test]
        public void Agent_Forgot_Password_By_Phone_Not_Found_Agent_Will_Fail()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakeSmsClient = Substitute.For<ISmsServiceClient>();

            var sut = new ForgotPasswordByPhoneCommandService(
                fakeAgentRepository,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator,
                fakeSmsClient);

            var command = new ForgotPasswordByPhoneCommand(
                Guid.NewGuid(),
                CultureInfo.GetCultureInfo("en-US"),
                "0900123456");

            fakeAgentRepository.GetAgentsByPhoneAsync(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(Array.Empty<AgentInfo>().ToAsyncEnumerable());

            Assert.ThrowsAsync<TGS.Services.Agent.Exceptions.AgentNotFoundException>(
                async () => await sut.GetAsync(command));
        }

        [Test]
        public void Agent_ForgotPassword_By_Mail_Not_Found_Agent_Will_Fail()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new ForgotPasswordByEmailCommandService(
                fakeAgentRepository,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var command = new ForgotPasswordByEmailCommand(
                Guid.NewGuid(),
                CultureInfo.GetCultureInfo("en-US"),
                "none@test.com");

            fakeAgentRepository.GetAgentsByEmailAsync(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(Array.Empty<AgentInfo>().ToAsyncEnumerable());

            Assert.ThrowsAsync<TGS.Services.Agent.Exceptions.AgentNotFoundException>(
                async () => await sut.GetAsync(command));
        }

        [Test]
        public async Task Agent_ForgotPassword_By_Mail_Should_Success()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new ForgotPasswordByEmailCommandService(
                fakeAgentRepository,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var command = new ForgotPasswordByEmailCommand(
                Guid.NewGuid(),
                CultureInfo.GetCultureInfo("en-US"),
                "agent@test.com");

            fakeAgentRepository.GetAgentsByEmailAsync(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(new[] {
                    new AgentInfo(
                        Guid.NewGuid(),
                        "agent",
                        "Agent1",
                        null,
                        Guid.NewGuid(),
                        null,
                        null,
                        null,
                        "test",
                        Array.Empty<byte>(),
                        Array.Empty<byte>(),
                        Array.Empty<byte>(),
                        Array.Empty<byte>(),
                        null,
                        DateTimeOffset.Now,
                        AgentStatus.Enabled,
                        AgentType.Cash,
                        AgentLevelType.Agent,
                        0M,
                        0)
                }.ToAsyncEnumerable());

            fakeValidateCodeGenerator.GenerateCode()
                .Returns("code");
            fakeValidateCodeGenerator.GenerateSecurityKey()
                .Returns("security");

            fakeValidateCodeOperator.SaveValidateDataAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<ValidateCodeData>())
                .Returns(true);

            var actual = await sut.GetAsync(command);

            _ = fakeValidateCodeOperator.Received(1)
                .SendEmailAsync(
                    Arg.Is(command.CultureInfo),
                    Arg.Is(command.Email),
                    Arg.Is("code"));

            Assert.AreEqual("security", actual);
        }

        [Test]
        public async Task Agent_ForgotPassword_By_Phone_Should_Success()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakeSmsClient = Substitute.For<ISmsServiceClient>();

            var sut = new ForgotPasswordByPhoneCommandService(
                fakeAgentRepository,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator,
                fakeSmsClient);

            var command = new ForgotPasswordByPhoneCommand(
                Guid.NewGuid(),
                CultureInfo.GetCultureInfo("en-US"),
                "0900123456");

            fakeAgentRepository.GetAgentsByPhoneAsync(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(new[] {
                    new AgentInfo(
                        Guid.NewGuid(),
                        "agent",
                        "Agent1",
                        null,
                        Guid.NewGuid(),
                        null,
                        null,
                        null,
                        "test",
                        Array.Empty<byte>(),
                        Array.Empty<byte>(),
                        Array.Empty<byte>(),
                        Array.Empty<byte>(),
                        null,
                        DateTimeOffset.Now,
                        AgentStatus.Enabled,
                        AgentType.Cash,
                        AgentLevelType.Agent,
                        0M,
                        0)
                }.ToAsyncEnumerable());

            fakeValidateCodeGenerator.GenerateCode()
                .Returns("code");
            fakeValidateCodeGenerator.GenerateSecurityKey()
                .Returns("security");

            fakeValidateCodeOperator.SaveValidateDataAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<ValidateCodeData>())
                .Returns(true);

            var actual = await sut.GetAsync(command);

            _ = fakeSmsClient.Received(1)
                .SendShortMessageAsync(
                    Arg.Is(command.Phone),
                    Arg.Is("code"),
                    Arg.Is(command.BrandId));

            Assert.AreEqual("security", actual);
        }

        [Test]
        public async Task Brand_Not_Use_Sms_Should_Use_Mail_ForgotPassword()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeForgotByMailService = Substitute.For<IGetService<ForgotPasswordByEmailCommand, string>>();
            var fakeForgotByPhoneService = Substitute.For<IGetService<ForgotPasswordByPhoneCommand, string>>();

            var sut = new ForgotPasswordCommandService(
                fakeBrandRepository,
                fakeForgotByMailService,
                fakeForgotByPhoneService);

            var command = new ForgotPasswordCommand(
                Guid.NewGuid(),
                CultureInfo.GetCultureInfo("en-US"),
                "tester@test.com");

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(command.BrandId))
                .Returns(new BrandInfo(
                    command.BrandId,
                    "Brand",
                    false,
                    false,
                    AgentDividendType.OneLayer,
                    null));

            await sut.GetAsync(command);

            _ = fakeForgotByMailService.Received(1)
                .GetAsync(Arg.Any<ForgotPasswordByEmailCommand>());

            _ = fakeForgotByPhoneService.DidNotReceive()
                .GetAsync(Arg.Any<ForgotPasswordByPhoneCommand>());
        }

        [Test]
        public async Task Brand_Use_Sms_Should_Use_Phone_ForgotPassword()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeForgotByMailService = Substitute.For<IGetService<ForgotPasswordByEmailCommand, string>>();
            var fakeForgotByPhoneService = Substitute.For<IGetService<ForgotPasswordByPhoneCommand, string>>();

            var sut = new ForgotPasswordCommandService(
                fakeBrandRepository,
                fakeForgotByMailService,
                fakeForgotByPhoneService);

            var command = new ForgotPasswordCommand(
                Guid.NewGuid(),
                CultureInfo.GetCultureInfo("en-US"),
                "tester@test.com");

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(command.BrandId))
                .Returns(new BrandInfo(
                    command.BrandId,
                    "Brand",
                    true,
                    true,
                    AgentDividendType.OneLayer,
                    null));

            await sut.GetAsync(command);

            _ = fakeForgotByPhoneService.Received(1)
                .GetAsync(Arg.Any<ForgotPasswordByPhoneCommand>());

            _ = fakeForgotByMailService.DidNotReceive()
                .GetAsync(Arg.Any<ForgotPasswordByEmailCommand>());
        }

        [Test]
        public void ChangePasswordRequestValidator_LengthCheck()
        {
            var sut = new ChangePasswordRequestValidator();

            var data = new ChangePasswordRequest { NewPassword = "12345" };

            var actual = sut.Validate(data);

            Assert.IsFalse(actual.IsValid);
        }
    }
}

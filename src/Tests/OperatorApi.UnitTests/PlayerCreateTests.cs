using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Validators;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Player;

namespace TGS.Tests.OperatorApi.UnitTests
{
    public class PlayerCreateTests
    {
        [Test]
        public async Task Register_Player_With_Existent_InvitationCode()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeSysRepository = Substitute.For<ISysRepository>();

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeSysRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        false)));

            var sut = new CreatePlayerCommandService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeSysRepository);

            var query = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1");

            // act
            var actual = await sut.GetAsync(query);

            // assert
            Assert.IsTrue(actual.IsNew);
        }

        [Test]
        public async Task Register_Player_With_No_InvitationCode()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeSysRepository = Substitute.For<ISysRepository>();

            fakeSysRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        false)));

            var sut = new CreatePlayerCommandService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeSysRepository);

            var password = "password";

            var query = new CreatePlayerCommand(brandId, "username", password, "CNY", null, false, "::1");

            // act
            var actual = await sut.GetAsync(query);
            // assert
            Assert.IsTrue(actual.IsNew);
        }

        [Test]
        public void Register_Player_With_Non_Existent_InvitationCode()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeSysRepository = Substitute.For<ISysRepository>();

            fakeSysRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        false)));

            var sut = new CreatePlayerCommandService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeSysRepository);

            var password = "password";
            var invitationCode = "NonExistentCode";

            var query = new CreatePlayerCommand(brandId, "username", password, "CNY", invitationCode, false, "::1");

            Assert.ThrowsAsync<InvalidInvitationCodeException>(async () => await sut.GetAsync(query));
        }

        [Test]
        public void Register_Player_With_Other_Brand_InvitationCode_Should_Throw_Error()
        {
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeSysRepository = Substitute.For<ISysRepository>();

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(default(Agent)));

            fakeSysRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        false)));

            var sut = new CreatePlayerCommandService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeSysRepository);

            var query = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1");

            // act
            Assert.ThrowsAsync<InvalidInvitationCodeException>(
                async () => await sut.GetAsync(query));
        }

        [Test]
        public void Verify_Player_Account_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest
            {
                Password = "123456",
                ConfirmPassword = "123456",
                Username = "123456789012345678901234567890123",
                AgentInvitationCode = string.Empty,
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Username);
        }

        [Test]
        public void Verify_Player_Account_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest
            {
                Password = "123456",
                ConfirmPassword = "123456",
                Username = "user",
                AgentInvitationCode = string.Empty,
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Username);
        }

        [Test]
        public void Verify_Player_Account_With_Special_Character()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest
            {
                Password = "123456",
                ConfirmPassword = "123456",
                Username = "abc123_",
                AgentInvitationCode = string.Empty,
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Username);
        }

        [Test]
        public void Verify_Player_Password_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest
            {
                Password = "123456789012345678901",
                ConfirmPassword = "123456789012345678901",
                Username = "username",
                AgentInvitationCode = string.Empty,
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }

        [Test]
        public void Verify_Player_Password_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest
            {
                Password = "1234",
                ConfirmPassword = "1234",
                Username = "username",
                AgentInvitationCode = string.Empty,
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }
    }
}
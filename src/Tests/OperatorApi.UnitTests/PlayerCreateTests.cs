using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Handlers.Player;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Services;
using TGS.Apis.OperatorApi.Validators;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;

namespace OperatorApi.UnitTests
{
    public class PlayerCreateTests
    {
        [Test]
        public void Verify_Player_Account_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
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
        public void Verify_Player_Account_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
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
        public void Verify_Player_Account_With_Special_Character()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
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
        public void Verify_Player_Password_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
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

        [Test]
        public void Verify_Player_Password_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();
            var request = new CreatePlayerRequest()
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
        public void Register_Player_With_Non_Existent_InvitationCode()
        {
            // arrange
            var playerCode = "testPlayerCode";
            var fakeHttpContext = new DefaultHttpContext();
            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData()
            {
                PlayerCode = playerCode
            });
            var fakeHttpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = fakeHttpContext
            };

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeInvitationCodeValidator = Substitute.For<IInvitationCodeValidator>();

            var sut = new CreatePlayerHandler
                (fakeHttpContextAccessor,
                fakePlayerOperations,
                fakeBrandOperations,
                fakeBrandCurrencyCodeMapper,
                fakePasswordHandler,
                fakeStatusValidator,
                fakeInvitationCodeValidator
                );
            string password = "password";
            string invitationCode = "NonExistentCode";

            var request = new CreatePlayerRequest()
            {
                AgentInvitationCode = invitationCode,
                Username = "username",
                Password = password,
                ConfirmPassword = password,
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };

            Assert.ThrowsAsync<InvitationCodeNotFoundException>(async () => await sut.Handle(request, default));
        }
    }
}
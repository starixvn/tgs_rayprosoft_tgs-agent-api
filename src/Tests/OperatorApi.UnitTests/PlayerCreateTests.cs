using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Validators;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Commands;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator.Services.Player;

namespace OperatorApi.UnitTests
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

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(new Agent());

            var sut = new CreatePlayerQueryService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations);

            var query = new CreatePlayerQuery(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false);

            // act
            var actual = await sut.GetAsync(query, default);

            // assert
            Assert.IsTrue(actual.IsNew);
        }

        [Test]
        public async Task Register_Player_With_Other_Brand_InvitationCode_Should_Throw_Error()
        {
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(default(Agent));

            var sut = new CreatePlayerQueryService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations);

            var query = new CreatePlayerQuery(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false);

            // act
            Assert.ThrowsAsync<InvalidInvitationCodeException>(
                async () => await sut.GetAsync(query, default));
        }

        [Test]
        public async Task Register_Player_With_No_InvitationCode()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var fakeFuncWalletQueries = Substitute.For<Func<IWalletQueries>>();
            var fakeFuncPlayerQueries = Substitute.For<Func<IPlayerQueries>>();
            var fakePlayerCommands = Substitute.For<IPlayerCommands>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();

            var BrandQueries = new BrandQueries(tgsRepository);

            var fakeBrandOperations = Substitute.For<IBrandOperations>();

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();

            var sut = new CreatePlayerQueryService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations);

            var password = "password";

            var query = new CreatePlayerQuery(brandId, "username", password, "CNY", null, false);

            // act
            var actual = await sut.GetAsync(query, default);
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

            var sut = new CreatePlayerQueryService(
                fakeStatusValidator,
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakeAgentOperations);
            var password = "password";
            var invitationCode = "NonExistentCode";

            var query = new CreatePlayerQuery(brandId, "username", password, "CNY", invitationCode, false);

            Assert.ThrowsAsync<InvalidInvitationCodeException>(async () => await sut.GetAsync(query, default));
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

using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Handlers.Player;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Validators;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Commands;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

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

            var sut = new CreatePlayerHandler
                (fakeHttpContextAccessor,
                fakePlayerOperations,
                fakeBrandOperations,
                fakeBrandCurrencyCodeMapper,
                fakePasswordHandler,
                fakeStatusValidator
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

            Assert.ThrowsAsync<InvalidInvitationCodeException>(async () => await sut.Handle(request, default));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode()
        {
            // arrange
            var playerCode = "playerCode";
            var invitationCode = "6rd4mq";
            var fakeHttpContext = new DefaultHttpContext();
            var brandId = Guid.NewGuid();
            fakeHttpContext.Items.Add(Consts.ContextItems.BrandId, brandId);
            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData()
            {
                PlayerCode = playerCode
            });
            var fakeHttpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = fakeHttpContext
            };

            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();
            var jsonSerializationProvider = new JsonSerializationProvider();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var fakeFuncWalletQueries = Substitute.For<Func<IWalletQueries>>();
            var fakeFuncPlayerQueries = Substitute.For<Func<IPlayerQueries>>();
            var fakePlayerCommands = Substitute.For<IPlayerCommands>();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();

            fakePlayerOperations
                .GetAgentAsync(Arg.Is(invitationCode))
                .Returns(new Agent());

            var BrandQueries = new BrandQueries(tgsRepository);

            var fakeTgsLog = Substitute.For<ITgsLog>();

            var fakeBrandOperations = Substitute.For<IBrandOperations>();

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();

            var sut = new CreatePlayerHandler
                (fakeHttpContextAccessor,
                fakePlayerOperations,
                fakeBrandOperations,
                fakeBrandCurrencyCodeMapper,
                fakePasswordHandler,
                fakeStatusValidator
                );
            string password = "password";

            var request = new CreatePlayerRequest()
            {
                AgentInvitationCode = invitationCode,
                Username = "username",
                Password = password,
                ConfirmPassword = password,
                CurrencyCode = "CNY",
                IsTestPlayer = false
            };
            // act
            var actual = await sut.Handle(request, default);
            // assert
            Assert.IsTrue(actual.IsNew);
        }
    }
}
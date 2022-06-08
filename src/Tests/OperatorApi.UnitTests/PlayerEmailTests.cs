using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Player;
using TGS.Services.Operator.ValidateCode;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerEmailTests
    {
        [Test]
        public void Commit_Init_Player_Email_If_Brand_Has_Same_Email_Should_Throw_Exception()
        {
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();
            var fakeCheckBrandEmailService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var brandId = Guid.NewGuid();
            var email = "test @mail.com";

            _ = fakeCheckBrandEmailService.GetAsync(
                    Arg.Is(new CheckBrandEmailQuery(brandId, email, playerCode)))
                .Returns(true);

            _ = fakeValidateCodeOperator.GetValidateCodeDataAsync<EmailValidateData>(
                    Arg.Is(validateCode),
                    Arg.Is(securityKey))
                .Returns(new EmailValidateData(email, playerCode));

            var command = new CommitInitPlayerEmailCommand(
                securityKey,
                validateCode,
                "127.0.0.1",
                "localhost",
                brandId);

            var sut = new CommitInitPlayerEmailCommandService(
                fakePlayerRepository,
                fakeCheckBrandEmailService,
                fakeValidateCodeOperator,
                fakeRedisCacheManager);

            _ = Assert.ThrowsAsync<BrandEmailDuplicateException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public void Commit_Init_Player_Email_If_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();
            var fakeCheckBrandEmailService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var brandId = Guid.NewGuid();
            var email = "test @mail.com";

            _ = fakeCheckBrandEmailService.GetAsync(
                    Arg.Is(new CheckBrandEmailQuery(brandId, email, playerCode)))
                .Returns(false);

            _ = fakeValidateCodeOperator.GetValidateCodeDataAsync<EmailValidateData>(
                    Arg.Is(validateCode),
                    Arg.Is(securityKey))
                .Returns((EmailValidateData)null);

            var command = new CommitInitPlayerEmailCommand(
                securityKey,
                validateCode,
                "127.0.0.1",
                "localhost",
                brandId);

            var sut = new CommitInitPlayerEmailCommandService(
                fakePlayerRepository,
                fakeCheckBrandEmailService,
                fakeValidateCodeOperator,
                fakeRedisCacheManager);

            _ = Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Commit_Init_Player_Email_Should_Success()
        {
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();
            var fakeCheckBrandEmailService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var brandId = Guid.NewGuid();
            var email = "test @mail.com";

            _ = fakeCheckBrandEmailService.GetAsync(
                    Arg.Is(new CheckBrandEmailQuery(brandId, email, playerCode)))
                .Returns(false);

            _ = fakeValidateCodeOperator.GetValidateCodeDataAsync<EmailValidateData>(
                    Arg.Is(validateCode),
                    Arg.Is(securityKey))
                .Returns(new EmailValidateData(email, playerCode));

            var command = new CommitInitPlayerEmailCommand(
                securityKey,
                validateCode,
                "127.0.0.1",
                "localhost",
                brandId);

            var sut = new CommitInitPlayerEmailCommandService(
                fakePlayerRepository,
                fakeCheckBrandEmailService,
                fakeValidateCodeOperator,
                fakeRedisCacheManager);

            await sut.ExecuteAsync(command).ConfigureAwait(false);

            _ = fakePlayerRepository
                .Received(1)
                .SetPlayerEmailAsync(
                    Arg.Is(playerCode),
                    Arg.Is(email));

            _ = fakePlayerRepository
                .Received(1)
                .UpdatePlayerStatusAsync(
                Arg.Is(playerCode),
                Arg.Is(PlayerStatus.Enabled));

            _ = fakePlayerRepository
                .Received(1)
                .PlayerRegisterIpAsync(
                    Arg.Is(playerCode),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository
                .Received(1)
                .PlayerLoginIpAsync(
                    Arg.Is(playerCode),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));

            _ = fakeRedisCacheManager
                  .Received(1)
                  .ClearCacheAsync(
                      Arg.Is<object>(x => x.ToString() == nameof(RedisCacheType.Player)),
                  Arg.Is<string>(x => x == playerCode),
                      Arg.Any<bool>());
        }

        [Test]
        public void Commit_Set_Player_Email_IF_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeCheckService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();

            var sut = new CommitSetPlayerEmailCommandService(
                fakeValidateCodeOperator,
                fakePlayerOperations,
                fakeCheckService);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var token = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var brandId = Guid.NewGuid();

            var command = new CommitSetPlayerEmailCommand(playerCode, token, validateCode, brandId);

            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(Arg.Any<string>(), Arg.Any<string>())
                .Returns((EmailValidateData)null);

            Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Commit_Set_Player_Email_Should_Success()
        {
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeCheckService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();

            var sut = new CommitSetPlayerEmailCommandService(
                fakeValidateCodeOperator,
                fakePlayerOperations,
                fakeCheckService);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var email = "test@raypro.com";
            var brandId = Guid.NewGuid();

            var command = new CommitSetPlayerEmailCommand(playerCode, securityKey, validateCode, brandId);

            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(Arg.Is(validateCode), Arg.Is(securityKey))
                .Returns(new EmailValidateData(email, playerCode));

            await sut.ExecuteAsync(command);

            _ = fakePlayerOperations
                .Received(1)
                .SetEmailAsync(Arg.Is(playerCode), Arg.Is(email), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public async Task Same_Brand_And_Same_Email_But_Same_Player_Should_Get_False()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            repository.Players.Add(new Core.DataServices.Tgs.Models.Player()
            {
                PlayerCode = "playerCode",
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                Email = "test@mail.com",
                DefaultWalletCurrencyCode = "CNY",
                Password = Array.Empty<byte>(),
                Salt = Array.Empty<byte>(),
                WithdrawPassword = Array.Empty<byte>(),
                WithdrawSalt = Array.Empty<byte>(),
                Username = "tester"
            });

            await repository.SaveChangesAsync();

            var sut = new CheckBrandEmailQueryService(repository);

            var actual = await sut.GetAsync(
                new CheckBrandEmailQuery(
                    new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                    "TEST@mail.com",
                    "playerCode1"));
            Assert.IsTrue(actual);
        }

        [Test]
        public async Task Same_Brand_And_Same_Email_Should_Get_True()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            repository.Players.Add(new Core.DataServices.Tgs.Models.Player()
            {
                PlayerCode = "playerCode",
                BrandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                Email = "test@mail.com",
                DefaultWalletCurrencyCode = "CNY",
                Password = Array.Empty<byte>(),
                Salt = Array.Empty<byte>(),
                WithdrawPassword = Array.Empty<byte>(),
                WithdrawSalt = Array.Empty<byte>(),
                Username = "tester"
            });

            await repository.SaveChangesAsync();

            var sut = new CheckBrandEmailQueryService(repository);

            var actual = await sut.GetAsync(
                new CheckBrandEmailQuery(
                    new Guid("4e86719f-243f-46f6-b010-5c758b7285af"),
                    "TEST@mail.com",
                    "playerCode1"));
            Assert.IsTrue(actual);
        }

        [Test]
        public void Set_Same_Email_In_Same_Brand_Should_Throw_Exception()
        {
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeCheckEmailService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();

            var sut = new CommitSetPlayerEmailCommandService(
                fakeValidateCodeOperator,
                fakePlayerOperations,
                fakeCheckEmailService);
            var brandId = Guid.NewGuid();

            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(Arg.Any<string>(), Arg.Any<string>())
                .Returns(new EmailValidateData("test@email.com", "playerCode"));

            fakeCheckEmailService
                .GetAsync(Arg.Any<CheckBrandEmailQuery>())
                .Returns(true);

            Assert.ThrowsAsync<BrandEmailDuplicateException>(async ()
                => await sut.ExecuteAsync(new CommitSetPlayerEmailCommand(
                    "playerCode",
                    "a0e4ec77-d05b-41c4-b7dd-dde8e46c27bc",
                    "909542",
                    brandId)));
        }

        [Test]
        public async Task Start_Set_Player_Email_Should_Not_Send_Confirm_Mail_If_Is_Register_Flow()
        {
            var fakeEmailClient = Substitute.For<IEmailClient>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new StartSetPlayerEmailCommandService(
                fakeEmailClient,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var playerCode = "testPlayerCode";
            var email = "test@raypro.com";
            var command = new StartSetPlayerEmailCommand(
                playerCode,
                email,
                CultureInfo.GetCultureInfo("en-US"),
                true);

            var validateCode = "123456";
            var expectedSecurityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            _ = fakeValidateCodeGenerator.GenerateSecurityKey().Returns(expectedSecurityKey);

            _ = fakeValidateCodeOperator
                .SaveValidateDataAsync(Arg.Is(validateCode), Arg.Is(expectedSecurityKey), Arg.Any<EmailValidateData>())
                .Returns(true);

            var actualSecurityKey = await sut.GetAsync(command);

            _ = fakeEmailClient
                .Received(0)
                .SendAsync(Arg.Any<SendEmailRequest>());

            _ = fakeValidateCodeGenerator
                .Received(0).GenerateCode();

            Assert.AreEqual(expectedSecurityKey, actualSecurityKey);
        }

        [Test]
        public async Task Start_Set_Player_Email_Should_Send_Confirm_Mail_If_Not_Register_Flow()
        {
            var fakeEmailClient = Substitute.For<IEmailClient>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new StartSetPlayerEmailCommandService(
                fakeEmailClient,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var playerCode = "testPlayerCode";
            var email = "test@raypro.com";
            var command = new StartSetPlayerEmailCommand(playerCode, email, CultureInfo.GetCultureInfo("en-US"), false);

            var validateCode = "123456";
            var expectedSecurityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            fakeValidateCodeGenerator.GenerateCode().Returns(validateCode);
            fakeValidateCodeGenerator.GenerateSecurityKey().Returns(expectedSecurityKey);

            fakeValidateCodeOperator
                .SaveValidateDataAsync(Arg.Is(validateCode), Arg.Is(expectedSecurityKey), Arg.Any<EmailValidateData>())
                .Returns(true);

            var expected = new
            {
                Receivers = new[] { email },
                Subject = "Verification Code",
                Content = $"Your ID verification code is : {validateCode}"
            }.ToExpectedObject();

            SendEmailRequest actual = null;
            fakeEmailClient
                .SendAsync(Arg.Do<SendEmailRequest>(msg =>
                {
                    actual = msg;
                }))
                .Returns(ValueTask.FromResult(new SendEmailResponse()));
            var actualSecurityKey = await sut.GetAsync(command);

            _ = fakeEmailClient
                .Received(1)
                .SendAsync(Arg.Any<SendEmailRequest>());

            expected.ShouldMatch(actual);
            Assert.AreEqual(expectedSecurityKey, actualSecurityKey);
        }
    }
}

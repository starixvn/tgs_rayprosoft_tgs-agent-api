using System;
using System.Globalization;
using System.Net.Mail;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.Player;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerEmailTests
    {
        [Test]
        public async Task Start_Set_Player_Email_Should_Send_Confirm_Mail()
        {
            var fakeEmailManager = Substitute.For<IEmailManager>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new StartSetPlayerEmailCommandService(
                fakeEmailManager,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var playerCode = "testPlayerCode";
            var email = "test@raypro.com";
            var command = new StartSetPlayerEmailCommand(playerCode, email, CultureInfo.GetCultureInfo("en-US"));

            var validateCode = "123456";
            var expectedSecurityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            fakeValidateCodeGenerator.GenerateCode().Returns(validateCode);
            fakeValidateCodeGenerator.GenerateSecurityKey().Returns(expectedSecurityKey);

            fakeValidateCodeOperator
                .SaveValidateDataAsync(Arg.Is(validateCode), Arg.Is(expectedSecurityKey), Arg.Any<EmailValidateData>())
                .Returns(true);

            var expected = new
            {
                To = new MailAddressCollection() {
                    new MailAddress(email)
                },
                Subject = "Verification Code",
                Content = $"Your ID verification code is : {validateCode}"
            }.ToExpectedObject();

            EmailPackage actual = null;
            fakeEmailManager
                .SendEmailAsync(Arg.Do<EmailPackage>(msg =>
                {
                    actual = msg;
                }))
                .Returns(Task.CompletedTask);
            var actualSecurityKey = await sut.GetAsync(command);

            _ = fakeEmailManager
                .Received(1)
                .SendEmailAsync(Arg.Any<EmailPackage>());

            expected.ShouldMatch(actual);
            Assert.AreEqual(expectedSecurityKey, actualSecurityKey);
        }

        [Test]
        public async Task Commit_Set_Player_Email_Should_Success()
        {
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeTgsRepository = Substitute.For<ITgsRepository>();
            var sut = new CommitSetPlayerEmailCommandService(
                fakeValidateCodeOperator,
                fakePlayerOperations,
                fakeTgsRepository);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var email = "test@raypro.com";

            var command = new CommitSetPlayerEmailCommand(playerCode, securityKey, validateCode);

            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(Arg.Is(validateCode), Arg.Is(securityKey))
                .Returns(new EmailValidateData(email, playerCode));

            await sut.ExecuteAsync(command);

            _ = fakePlayerOperations
                .Received(1)
                .SetEmailAsync(Arg.Is(playerCode), Arg.Is(email), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public void Commit_Set_Player_Email_IF_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeTgsRepository = Substitute.For<ITgsRepository>();

            var sut = new CommitSetPlayerEmailCommandService(
                fakeValidateCodeOperator,
                fakePlayerOperations,
                fakeTgsRepository);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var token = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            var command = new CommitSetPlayerEmailCommand(playerCode, token, validateCode);

            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(Arg.Any<string>(), Arg.Any<string>())
                .Returns((EmailValidateData)null);

            Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public void Commit_Set_Player_Email_IF_Mail_Duplicate_Should_Throw_Exception()
        {
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var email = "same@email.com";
            var playerCode = "testPlayerCode";
            repository.Players.Add(new Player()
            {
                Email = email,
                PlayerCode = playerCode
            });
            repository.SaveChangesAsync();

            var sut = new CommitSetPlayerEmailCommandService(
                fakeValidateCodeOperator,
                fakePlayerOperations,
                repository);

            var validateCode = "123456";
            var token = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            var command = new CommitSetPlayerEmailCommand(playerCode, token, validateCode);

            fakeValidateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(Arg.Is(validateCode), Arg.Is(securityKey))
                .Returns(new EmailValidateData(email, playerCode));

            Assert.ThrowsAsync<EmailAlreadySetException>(async () => await sut.ExecuteAsync(command));
        }
    }
}
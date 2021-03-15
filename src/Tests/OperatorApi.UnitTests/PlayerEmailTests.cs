using System.Globalization;
using System.Net.Mail;
using System.Threading.Tasks;
using ExpectedObjects;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Operator;
using TGS.Services.Operator.PlayerEmail;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerEmailTests
    {
        [Test]
        public async Task Start_Set_Player_Email_Should_Send_Confirm_MailAsync()
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
            var token = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            fakeValidateCodeGenerator.GenerateCode().Returns(validateCode);
            fakeValidateCodeGenerator.GenerateToken().Returns(token);

            fakeValidateCodeOperator
                .SaveValidateDataAsync(Arg.Is(validateCode), Arg.Is(token), Arg.Any<ValidateCodeData>())
                .Returns(true);

            var expected = new
            {
                To = new MailAddressCollection() {
                    new MailAddress(email)
                },
                Subject = "Verification Code",
                Content = $"Your ID verification code is : {validateCode}"
            }.ToExpectedObject();

            Message actual = null;
            fakeEmailManager
                .SendEmailAsync(Arg.Do<Message>(msg =>
                {
                    actual = msg;
                }))
                .Returns(Task.CompletedTask);
            await sut.GetAsync(command);

            _ = fakeEmailManager
                .Received(1)
                .SendEmailAsync(Arg.Any<Message>());

            expected.ShouldMatch(actual);
        }
    }
}

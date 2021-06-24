using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Agent.Services.Agent;
using TGS.Services.Agent.ValidateCode;

namespace Agent.UnitTests
{
    [TestFixture]
    public class AgentEmailTests
    {
        [Test]
        public async Task Start_Set_Agent_Email_Should_Send_Confirm_Mail()
        {
            var fakeEmailClient = Substitute.For<IEmailClient>();
            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new StartSetAgentEmailCommandService(
                fakeEmailClient,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var agentId = Guid.NewGuid();
            var email = "test@raypro.com";
            var command = new StartSetAgentEmailCommand(
                agentId,
                email,
                CultureInfo.GetCultureInfo("en-US"));

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

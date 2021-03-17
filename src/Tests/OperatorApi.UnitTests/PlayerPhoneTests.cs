using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.PlayerPhone;
using TGS.SmsService.Client;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerPhoneTests
    {
        [Test]
        public async Task Start_Set_Player_Phone_Should_Send_Confirm_Message()
        {
            var moqPlayerOperations = new Mock<IPlayerOperations>();
            var moqValidateCodeGenerator = new Mock<IValidateCodeGenerator>();
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqSmsServiceClient = new Mock<ISmsServiceClient>();

            var sut = new StartSetPlayerPhoneCommandService(
                moqPlayerOperations.Object,
                moqValidateCodeGenerator.Object,
                moqValidateCodeOperator.Object,
                moqSmsServiceClient.Object);

            var phone = "0988123456";
            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                PhoneAuthentication = true,
                DefaultWalletCurrencyCode = "CNY"
            };

            var command = new StartSetPlayerPhoneCommand(player.PlayerCode, phone, CultureInfo.GetCultureInfo("en-US"));

            var validateCode = "123456";
            var expectedSecurityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            moqPlayerOperations
                .Setup(fake => fake.GetPlayerByCodeAsync(It.Is<string>(x => x == player.PlayerCode)))
                .Returns(Task.FromResult(player));

            moqValidateCodeGenerator.Setup(fake => fake.GenerateCode()).Returns(validateCode);
            moqValidateCodeGenerator.Setup(fake => fake.GenerateSecurityKey()).Returns(expectedSecurityKey);

            moqValidateCodeOperator
                .Setup(
                    fake => fake.SaveValidateDataAsync(
                        It.Is<string>(x => x == validateCode),
                        It.Is<string>(x => x == expectedSecurityKey),
                        It.IsAny<PhoneValidateData>()))
                .Returns(ValueTask.FromResult(true));

            var expectedShortMessage = $"Your ID verification code is : {validateCode}";

            var actualSecurityKey = await sut.GetAsync(command);

            moqSmsServiceClient
                .Verify(
                    smsClient => smsClient.SendShortMessageAsync(
                        It.Is<string>(x => x == phone),
                        It.Is<string>(x => x == expectedShortMessage),
                        It.IsAny<string>(),
                        It.IsAny<Guid>()),
                    Times.Exactly(1));
            Assert.AreEqual(expectedSecurityKey, actualSecurityKey);
        }
    }
}

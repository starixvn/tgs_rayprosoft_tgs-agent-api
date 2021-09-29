using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Player;
using TGS.Services.Operator.ValidateCode;
using TGS.SmsService.Client;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerPhoneTests
    {
        [Test]
        public void Commit_Set_Player_Phone_If_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqCacheManager = new Mock<IRedisCacheManager>();

            var sut = new CommitSetPlayerPhoneCommandService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            var command = new CommitSetPlayerPhoneCommand(playerCode, securityKey, validateCode);

            moqValidateCodeOperator
                .Setup(
                    fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(ValueTask.FromResult<PhoneValidateData>(null));

            Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Commit_Set_Player_Phone_Should_Success()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqCacheManager = new Mock<IRedisCacheManager>();

            var sut = new CommitSetPlayerPhoneCommandService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var phone = "0988123456";
            var areaCode = "886";

            var command = new CommitSetPlayerPhoneCommand(playerCode, securityKey, validateCode);

            moqValidateCodeOperator
                .Setup(
                    fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                        It.Is<string>(x => x == validateCode),
                        It.Is<string>(x => x == securityKey)))
                .Returns(ValueTask.FromResult(new PhoneValidateData(
                    areaCode,
                    phone,
                    playerCode)));

            await sut.ExecuteAsync(command);

            moqPlayerRepository.Verify(
                fake => fake.SetPlayerPhoneAsync(
                    It.Is<string>(x => x == playerCode),
                    It.Is<string>(x => x == areaCode),
                    It.Is<string>(x => x == phone)),
                Times.Exactly(1));
        }

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

            var areaCode = "886";
            var phone = "0988123456";
            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                PhoneAuthentication = true,
                DefaultWalletCurrencyCode = "CNY"
            };

            var command = new StartSetPlayerPhoneCommand(
                player.PlayerCode,
                areaCode,
                phone,
                CultureInfo.GetCultureInfo("en-US"));

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
                        It.Is<string>(x => x == $"{areaCode}{phone}"),
                        It.Is<string>(x => x == expectedShortMessage),
                        It.IsAny<Guid>()),
                    Times.Exactly(1));
            Assert.AreEqual(expectedSecurityKey, actualSecurityKey);
        }

        [Test]
        public void Commit_Init_Player_Phone_If_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqPlayerOperations = new Mock<IPlayerOperations>();
            var moqCacheManager = new Mock<IRedisCacheManager>();

            var sut = new CommitPlayerInitPhoneService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object);

            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            var command = new CommitPlayerInitPhoneCommand(securityKey, validateCode);

            moqValidateCodeOperator
                .Setup(
                    fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(ValueTask.FromResult<PhoneValidateData>(null));

            Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Commit_Init_Player_Phone_Should_Success()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqPlayerOperations = new Mock<IPlayerOperations>();
            var moqCacheManager = new Mock<IRedisCacheManager>();

            var sut = new CommitPlayerInitPhoneService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var phone = "0988123456";
            var areaCode = "886";

            var command = new CommitPlayerInitPhoneCommand(securityKey, validateCode);

            moqValidateCodeOperator
                .Setup(
                    fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                        It.Is<string>(x => x == validateCode),
                        It.Is<string>(x => x == securityKey)))
                .Returns(ValueTask.FromResult(new PhoneValidateData(
                    areaCode,
                    phone,
                    playerCode)));

            await sut.ExecuteAsync(command);

            moqPlayerRepository.Verify(
                fake => fake.SetPlayerPhoneAsync(
                    It.Is<string>(x => x == playerCode),
                    It.Is<string>(x => x == areaCode),
                    It.Is<string>(x => x == phone)),
                Times.Exactly(1));

            moqPlayerRepository.Verify(
                fake => fake.UpdatePlayerStatusAsync(
                    It.Is<string>(x => x == playerCode),
                    It.Is<PlayerStatus>(x => x == PlayerStatus.Enabled)),
                Times.Exactly(1));

            moqCacheManager.Verify(
                fake => fake.ClearCacheAsync(
                    It.Is<object>(x => x.ToString() == nameof(RedisCacheType.Player)),
                    It.Is<object>(x => x.ToString() == playerCode),
                    It.Is<bool>(x => x == false)),
                Times.Exactly(1));
        }
    }
}
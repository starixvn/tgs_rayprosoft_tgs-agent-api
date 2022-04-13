using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Player;
using TGS.Services.Operator.ValidateCode;
using TGS.SmsService.Client;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerPhoneTests
    {
        [Test]
        public void Commit_Init_Player_Phone_If_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqPlayerOperations = new Mock<IPlayerOperations>();
            var moqCacheManager = new Mock<IRedisCacheManager>();

            var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

            var sut = new CommitPlayerInitPhoneService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object,
                moqCheckService.Object);

            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            var command = new CommitPlayerInitPhoneCommand(
                securityKey,
                validateCode,
                "127.0.0.1",
                "localhost",
                Guid.NewGuid());

            moqValidateCodeOperator
                .Setup(
                    fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(ValueTask.FromResult<PhoneValidateData>(null));

            Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Commit_Init_Player_Phone_Should_Set_RegisteredOn()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqPlayerOperations = new Mock<IPlayerOperations>();
            var moqCacheManager = new Mock<IRedisCacheManager>();
            var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

            var sut = new CommitPlayerInitPhoneService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object,
                moqCheckService.Object);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var phone = "0988123456";
            var areaCode = "886";

            var command = new CommitPlayerInitPhoneCommand(
                securityKey,
                validateCode,
                "127.0.0.1",
                "localhost",
                Guid.NewGuid());

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
                fake => fake.PlayerRegisterIpAsync(
                    It.Is<string>(x => x == playerCode),
                    It.Is<string>(x => x == command.IpAddress),
                    It.Is<string>(x => x == command.HostName),
                    It.IsAny<DateTimeOffset>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Commit_Init_Player_Phone_Should_Success()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqPlayerOperations = new Mock<IPlayerOperations>();
            var moqCacheManager = new Mock<IRedisCacheManager>();
            var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

            var sut = new CommitPlayerInitPhoneService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object,
                moqCheckService.Object);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var phone = "0988123456";
            var areaCode = "886";

            var command = new CommitPlayerInitPhoneCommand(
                securityKey,
                validateCode,
                "127.0.0.1",
                "localhost",
                Guid.NewGuid());

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

        [Test]
        public void Commit_Set_Player_Phone_If_Not_Verify_ValidateCode_Should_Throw_Exception()
        {
            var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
            var moqPlayerRepository = new Mock<IPlayerRepository>();
            var moqCacheManager = new Mock<IRedisCacheManager>();
            var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

            var sut = new CommitSetPlayerPhoneCommandService(                
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object,
                moqCheckService.Object);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

            var command = new CommitSetPlayerPhoneCommand(Guid.NewGuid(), playerCode, securityKey, validateCode);

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
            var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

            var sut = new CommitSetPlayerPhoneCommandService(
                moqValidateCodeOperator.Object,
                moqPlayerRepository.Object,
                moqCacheManager.Object,
                moqCheckService.Object);

            var playerCode = "testPlayerCode";
            var validateCode = "123456";
            var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
            var phone = "0988123456";
            var areaCode = "886";

            var command = new CommitSetPlayerPhoneCommand(Guid.NewGuid(), playerCode, securityKey, validateCode);

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
        public async Task Same_Brand_And_Different_Phone_And_Same_AreaCode_Should_Get_False()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var fakeTgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var brandId = Guid.Parse("1D46DDD7-EB42-43A0-CB2F-724E59622CA5");
            var playerCode = "playerCode";
            var areaCode = "+852";
            var phone = "66666666";

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = brandId,
                Phone = phone,
                AreaCode = areaCode,
                PhoneAuthentication = true,
                DefaultWalletCurrencyCode = "CNY"
            };

            await fakeTgsRepository.Players.AddAsync(player);
            await fakeTgsRepository.SaveChangesAsync();

            var sut = new CheckBrandPhoneQueryService(fakeTgsRepository);

            var actual = await sut.GetAsync(new CheckBrandPhoneQuery(brandId, playerCode, areaCode, "88888888"));

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task Same_Brand_And_Same_Phone_And_Same_AreaCode_Should_Get_True()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var fakeTgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var brandId = Guid.Parse("1D46DDD7-EB42-43A0-CB2F-724E59622CA5");
            var playerCode = "playerCode";
            var areaCode = "+852";
            var phone = "66666666";

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = brandId,
                Phone = phone,
                AreaCode = areaCode,
                PhoneAuthentication = true,
                DefaultWalletCurrencyCode = "CNY"
            };

            await fakeTgsRepository.Players.AddAsync(player);
            await fakeTgsRepository.SaveChangesAsync();

            var sut = new CheckBrandPhoneQueryService(fakeTgsRepository);

            var actual = await sut.GetAsync(new CheckBrandPhoneQuery(brandId, playerCode, areaCode, phone));

            Assert.IsTrue(actual);
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
    }
}

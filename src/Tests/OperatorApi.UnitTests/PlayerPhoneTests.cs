using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NSubstitute;
using NUnit.Framework;
using Tgs.BrandCertificate.Clients;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Player;
using TGS.Services.Operator.ValidateCode;
using TGS.SmsService.Client;

namespace TGS.Tests.OperatorApi.UnitTests;

[TestFixture]
public class PlayerPhoneTests
{
    [Test]
    public async Task Commit_Init_Player_Phone_If_Brand_Has_EmailRegisterFlow_Should_Set_Phone_Only()
    {
        var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
        var moqPlayerRepository = new Mock<IPlayerRepository>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var moqCacheManager = new Mock<IRedisCacheManager>();
        var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

        var sut = new CommitInitPlayerPhoneCommandService(
            moqValidateCodeOperator.Object,
            fakeBrandServiceClient,
            moqPlayerRepository.Object,
            moqCacheManager.Object,
            moqCheckService.Object);

        var playerCode = "testPlayerCode";
        var validateCode = "123456";
        var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
        var phone = "0988123456";
        var areaCode = "886";

        var brandInfo = new Tgs.BrandCertificate.Clients.BrandInfo
        {
            Id = Guid.NewGuid(),
            Code = "code",
            Currency = "CNY",
            EmailRegisterFlowEnabled = true,
            SmsRegisterFlowEnabled = true
        };

        var command = new CommitInitPlayerPhoneCommand(
            securityKey,
            validateCode,
            "127.0.0.1",
            "localhost",
            brandInfo.Id);

        moqValidateCodeOperator
            .Setup(
                fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                    It.Is<string>(x => x == validateCode),
                    It.Is<string>(x => x == securityKey)))
            .Returns(ValueTask.FromResult(new PhoneValidateData(
                areaCode,
                phone,
                playerCode)));

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandInfo.Id))
            .Returns(ValueTask.FromResult(brandInfo));

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
            Times.Never);

        moqCacheManager.Verify(
            fake => fake.ClearCacheAsync(
                It.Is<object>(x => x.ToString() == nameof(RedisCacheType.Player)),
                It.Is<object>(x => x.ToString() == playerCode),
                It.Is<bool>(x => x == false)),
            Times.Never);

        moqPlayerRepository.Verify(
            fake => fake.PlayerRegisterIpAsync(
                It.Is<string>(x => x == playerCode),
                It.Is<string>(x => x == command.IpAddress),
                It.Is<string>(x => x == command.HostName),
                It.IsAny<DateTimeOffset>()),
            Times.Never);

        moqPlayerRepository.Verify(
            fake => fake.PlayerLoginIpAsync(
                It.Is<string>(x => x == playerCode),
                It.Is<string>(x => x == command.IpAddress),
                It.Is<string>(x => x == command.HostName)),
            Times.Never);
    }

    [Test]
    public async Task Commit_Init_Player_Phone_If_Brand_Has_No_SmsRegisterFlow_Should_Set_Phone_Only()
    {
        var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
        var moqPlayerRepository = new Mock<IPlayerRepository>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var moqCacheManager = new Mock<IRedisCacheManager>();
        var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

        var sut = new CommitInitPlayerPhoneCommandService(
            moqValidateCodeOperator.Object,
            fakeBrandServiceClient,
            moqPlayerRepository.Object,
            moqCacheManager.Object,
            moqCheckService.Object);

        var playerCode = "testPlayerCode";
        var validateCode = "123456";
        var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
        var phone = "0988123456";
        var areaCode = "886";

        var brandInfo = new Tgs.BrandCertificate.Clients.BrandInfo
        {
            Id = Guid.NewGuid(),
            Code = "code",
            Currency = "CNY",
            EmailRegisterFlowEnabled = false,
            SmsRegisterFlowEnabled = false
        };

        var command = new CommitInitPlayerPhoneCommand(
            securityKey,
            validateCode,
            "127.0.0.1",
            "localhost",
            brandInfo.Id);

        moqValidateCodeOperator
            .Setup(
                fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                    It.Is<string>(x => x == validateCode),
                    It.Is<string>(x => x == securityKey)))
            .Returns(ValueTask.FromResult(new PhoneValidateData(
                areaCode,
                phone,
                playerCode)));

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandInfo.Id))
            .Returns(ValueTask.FromResult(brandInfo));

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
            Times.Never);

        moqCacheManager.Verify(
            fake => fake.ClearCacheAsync(
                It.Is<object>(x => x.ToString() == nameof(RedisCacheType.Player)),
                It.Is<object>(x => x.ToString() == playerCode),
                It.Is<bool>(x => x == false)),
            Times.Never);

        moqPlayerRepository.Verify(
            fake => fake.PlayerRegisterIpAsync(
                It.Is<string>(x => x == playerCode),
                It.Is<string>(x => x == command.IpAddress),
                It.Is<string>(x => x == command.HostName),
                It.IsAny<DateTimeOffset>()),
            Times.Never);

        moqPlayerRepository.Verify(
            fake => fake.PlayerLoginIpAsync(
                It.Is<string>(x => x == playerCode),
                It.Is<string>(x => x == command.IpAddress),
                It.Is<string>(x => x == command.HostName)),
            Times.Never);
    }

    [Test]
    public async Task Commit_Init_Player_Phone_If_Brand_Use_SmsRegisterFlow_Should_Success()
    {
        var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
        var moqPlayerRepository = new Mock<IPlayerRepository>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var moqCacheManager = new Mock<IRedisCacheManager>();
        var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

        var sut = new CommitInitPlayerPhoneCommandService(
            moqValidateCodeOperator.Object,
            fakeBrandServiceClient,
            moqPlayerRepository.Object,
            moqCacheManager.Object,
            moqCheckService.Object);

        var playerCode = "testPlayerCode";
        var validateCode = "123456";
        var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
        var phone = "0988123456";
        var areaCode = "886";

        var brandInfo = new Tgs.BrandCertificate.Clients.BrandInfo
        {
            Id = Guid.NewGuid(),
            Code = "code",
            Currency = "CNY",
            SmsRegisterFlowEnabled = true
        };

        var command = new CommitInitPlayerPhoneCommand(
            securityKey,
            validateCode,
            "127.0.0.1",
            "localhost",
            brandInfo.Id);

        moqValidateCodeOperator
            .Setup(
                fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                    It.Is<string>(x => x == validateCode),
                    It.Is<string>(x => x == securityKey)))
            .Returns(ValueTask.FromResult(new PhoneValidateData(
                areaCode,
                phone,
                playerCode)));

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandInfo.Id))
            .Returns(ValueTask.FromResult(brandInfo));

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

        moqPlayerRepository.Verify(
            fake => fake.PlayerRegisterIpAsync(
                It.Is<string>(x => x == playerCode),
                It.Is<string>(x => x == command.IpAddress),
                It.Is<string>(x => x == command.HostName),
                It.IsAny<DateTimeOffset>()),
            Times.Exactly(1));

        moqPlayerRepository.Verify(
            fake => fake.PlayerLoginIpAsync(
                It.Is<string>(x => x == playerCode),
                It.Is<string>(x => x == command.IpAddress),
                It.Is<string>(x => x == command.HostName)),
            Times.Exactly(1));
    }

    [Test]
    public void Commit_Init_Player_Phone_If_Not_Verify_ValidateCode_Should_Throw_Exception()
    {
        var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
        var moqPlayerRepository = new Mock<IPlayerRepository>();
        var moqBrandRepository = new Mock<IBrandRepository>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var moqCacheManager = new Mock<IRedisCacheManager>();

        var moqCheckService = new Mock<IGetService<CheckBrandPhoneQuery, bool>>();

        var sut = new CommitInitPlayerPhoneCommandService(
            moqValidateCodeOperator.Object,
            fakeBrandServiceClient,
            moqPlayerRepository.Object,
            moqCacheManager.Object,
            moqCheckService.Object);

        var validateCode = "123456";
        var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

        var brandInfo = new Tgs.BrandCertificate.Clients.BrandInfo
        {
            Id = Guid.NewGuid(),
            Code = "code",
            Currency = "CNY",
            SmsRegisterFlowEnabled = true
        };

        var command = new CommitInitPlayerPhoneCommand(
            securityKey,
            validateCode,
            "127.0.0.1",
            "localhost",
            brandInfo.Id);

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandInfo.Id))
            .Returns(ValueTask.FromResult(brandInfo));

        moqValidateCodeOperator
            .Setup(
                fake => fake.GetValidateCodeDataAsync<PhoneValidateData>(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(ValueTask.FromResult<PhoneValidateData>(null));

        Assert.ThrowsAsync<ValidateCodeNotVerifyException>(async () => await sut.ExecuteAsync(command));
    }

    [Test]
    public void Commit_Set_Player_Phone_If_Brand_Has_Same_Phone_Should_Throw_Exception()
    {
        var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeCacheManager = Substitute.For<IRedisCacheManager>();
        var fakeCheckService = Substitute.For<IGetService<CheckBrandPhoneQuery, bool>>();

        var sut = new CommitSetPlayerPhoneCommandService(
            fakeValidateCodeOperator,
            fakePlayerRepository,
            fakeCacheManager,
            fakeCheckService);

        var playerCode = "testPlayerCode";
        var validateCode = "123456";
        var securityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";
        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var areaCode = "852";
        var phone = "67329548";

        var command = new CommitSetPlayerPhoneCommand(brandId, playerCode, securityKey, validateCode);

        fakeValidateCodeOperator
            .GetValidateCodeDataAsync<PhoneValidateData>(validateCode, Arg.Any<string>())
            .Returns(new PhoneValidateData(areaCode, phone, playerCode));

        fakeCheckService
            .GetAsync(new CheckBrandPhoneQuery(brandId, playerCode, areaCode, phone))
            .Returns(true);

        Assert.ThrowsAsync<BrandPhoneDuplicateException>(async () => await sut.ExecuteAsync(command));
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
    public async Task Same_Brand_And_Different_Phone_And_Same_AreaCode_Should_Get_True()
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
            DefaultWalletCurrencyCode = "CNY",
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>()
        };

        await fakeTgsRepository.Players.AddAsync(player);
        await fakeTgsRepository.SaveChangesAsync();

        var sut = new CheckBrandPhoneQueryService(fakeTgsRepository);

        var actual = await sut.GetAsync(new CheckBrandPhoneQuery(brandId, playerCode, areaCode, "88888888"));

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Same_Brand_And_Same_Phone_And_Same_AreaCode_Should_Get_False()
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
            DefaultWalletCurrencyCode = "CNY",
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>()
        };

        await fakeTgsRepository.Players.AddAsync(player);
        await fakeTgsRepository.SaveChangesAsync();

        var sut = new CheckBrandPhoneQueryService(fakeTgsRepository);

        var actual = await sut.GetAsync(new CheckBrandPhoneQuery(brandId, playerCode, areaCode, phone));

        Assert.IsFalse(actual);
    }

    [Test]
    public async Task Start_Set_Player_Phone_Should_Not_Send_Confirm_Message_When_Sms_Disabled()
    {
        var moqPlayerRepository = new Mock<IPlayerRepository>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var moqValidateCodeGenerator = new Mock<IValidateCodeGenerator>();
        var moqValidateCodeOperator = new Mock<IValidateCodeOperator>();
        var moqSmsServiceClient = new Mock<ISmsServiceClient>();

        var sut = new StartSetPlayerPhoneCommandService(
            moqPlayerRepository.Object,
            fakeBrandServiceClient,
            moqValidateCodeGenerator.Object,
            moqValidateCodeOperator.Object,
            moqSmsServiceClient.Object);

        var areaCode = "886";
        var phone = "0988123456";
        var player = new PlayerInfo(
            "abcdef123456",
            Guid.NewGuid(),
            0M,
            false,
            false,
            null,
            null,
            null,
            PlayerStatus.Enabled,
            "Tester",
            "Tester",
            null,
            1,
            null,
            null,
            false);

        var command = new StartSetPlayerPhoneCommand(
            player.PlayerCode,
            areaCode,
            phone,
            CultureInfo.GetCultureInfo("en-US"),
            false);

        var validateCode = "123456";
        var expectedSecurityKey = "3F8E5681-50C8-4F2F-8379-3AA6EF2C6064";

        moqPlayerRepository
            .Setup(fake => fake.GetPlayerInfoAsync(It.Is<string>(x => x == player.PlayerCode)))
            .Returns(ValueTask.FromResult(player));

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(player.BrandId))
            .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
            {
                Id = player.BrandId,
                Code = "Test",
                Currency = "CNY",
                SmsEnabled = false,
                SmsRegisterFlowEnabled = true
            });

        moqValidateCodeGenerator
            .Setup(fake => fake.GenerateCode())
            .Returns(validateCode);
        moqValidateCodeGenerator
            .Setup(fake => fake.GenerateSecurityKey())
            .Returns(expectedSecurityKey);

        moqValidateCodeOperator
            .Setup(fake => fake.SaveValidateDataAsync(
                It.Is<string>(x => x == validateCode),
                It.Is<string>(x => x == expectedSecurityKey),
                It.IsAny<PhoneValidateData>()))
            .Returns(ValueTask.FromResult(true));

        var expectedShortMessage = $"Your ID verification code is : {validateCode}";

        var actualSecurityKey = await sut.GetAsync(command);

        moqSmsServiceClient.Verify(
            smsClient => smsClient.SendShortMessageAsync(
                It.Is<string>(x => x == $"{areaCode}{phone}"),
                It.Is<string>(x => x == expectedShortMessage),
                It.IsAny<Guid>()),
            Times.Exactly(0));

        Assert.AreEqual(expectedSecurityKey, actualSecurityKey);
    }

    [Test]
    public async Task CheckPhone_If_Not_Input_PlayerCode()
    {
        var brandId = Guid.Parse("1D46DDD7-EB42-43A0-CB2F-724E59622CA5");
        var areaCode = "+852";
        var phone = "66666666";

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

        dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

        var fakeTgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

        fakeTgsRepository.Players.Add(new Player
        {
            PlayerCode = "player1",
            BrandId = brandId,
            AreaCode = "+852",
            Phone = "66666666",
            DefaultWalletCurrencyCode = "CNY",
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            Username = "Player1",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>()
        });

        fakeTgsRepository.SaveChanges();

        var sut = new CheckBrandPhoneQueryService(fakeTgsRepository);

        var query = new CheckBrandPhoneQuery(brandId, null, areaCode, phone);

        var actual = await sut.GetAsync(query);

        Assert.IsFalse(actual);
    }
}

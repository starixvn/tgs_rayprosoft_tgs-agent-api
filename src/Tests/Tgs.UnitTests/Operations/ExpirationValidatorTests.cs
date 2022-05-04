#nullable enable

using System;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.GameProviders.Repositories.Models;
using TGS.Services.GameOperations;
using TGS.Services.GameOperations.Validators;
using TGS.Tests.TestsCommon;

namespace TGS.UnitTests.Operations;

[TestFixture]
[Category(Named.Unit)]
[Parallelizable]
public class ExpirationValidatorTests
{
    //[TestCase(null)]
    //[TestCase(PlatformType.Desktop)]
    //public async Task ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_LoginRecord_Is_Expired(
    //    PlatformType? platform)
    //{
    //    var fixture = new Fixture();
    //    var brandId = fixture.Create<Guid>();
    //    var playerCode = fixture.Create<string>();

    //    var results = new GetPlayerLoginRecordResult[]
    //    {
    //        new()
    //        {
    //            BrandId = brandId,
    //            CreatedOn = DateTimeOffset.UtcNow.AddHours(-23),
    //            PlatformType = platform ?? PlatformType.Desktop,
    //            Status = LoginStatus.Enabled,
    //            ExpiredOn = DateTimeOffset.UtcNow.AddHours(-1)
    //        }
    //    };

    //    _ = _playerQueriesMock.Setup(x =>
    //            x.GetPlayerLoginsRecordsInOneDayAsync(playerCode, LoginStatus.Enabled))
    //        .ReturnsAsync(results);

    //    var sut = new ExpirationValidator(TODO);

    //    Func<Task> action = async () =>
    //        await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

    //    _ = await action.Should()
    //        .ThrowAsync<PlayerLoginExpiredException>()
    //        .WithMessage($"Player({playerCode}) login token is expired");
    //}

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_And_Platform_No_Throws_When_Is_Valid()
    {
        var brandId = Guid.NewGuid();
        var playerCode = "player1";
        var platformType = PlatformType.Desktop;

        var playerLoginRecord = new PlayerLoginRecordModel
        {
            Id = Guid.NewGuid(),
            BrandId = brandId,
            PlayerCode = playerCode,
            PlatformType = platformType,
            Status = LoginStatus.Enabled,
            CreatedOn = DateTimeOffset.UtcNow,
            ExpiredOn = DateTimeOffset.UtcNow.AddHours(12)
        };

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode, platformType))
            .ReturnsAsync(playerLoginRecord);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        Assert.DoesNotThrowAsync(async ()
            => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platformType).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_And_Platform_Throws_Exception_When_Is_Expired()
    {
        var brandId = Guid.NewGuid();
        var playerCode = "player1";
        var platformType = PlatformType.Desktop;

        var playerLoginRecord = new PlayerLoginRecordModel
        {
            Id = Guid.NewGuid(),
            BrandId = brandId,
            PlayerCode = playerCode,
            PlatformType = PlatformType.Desktop,
            Status = LoginStatus.Enabled,
            CreatedOn = DateTimeOffset.UtcNow.AddHours(-24),
            ExpiredOn = DateTimeOffset.UtcNow.AddHours(-12)
        };

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode, platformType))
            .ReturnsAsync(playerLoginRecord);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(async ()
            => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platformType).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_And_Platform_Throws_Exception_When_Is_Null()
    {
        var playerCode = "player1";
        var platformType = PlatformType.Desktop;

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode, platformType))
            .ReturnsAsync((PlayerLoginRecordModel?)null);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(async ()
              => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platformType).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_Only_No_Throws_Exception_When_Is_Expired()
    {
        var brandId = Guid.NewGuid();
        var playerCode = "player1";

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        var playerLoginRecord = new PlayerLoginRecordModel
        {
            Id = Guid.NewGuid(),
            BrandId = brandId,
            PlayerCode = playerCode,
            PlatformType = PlatformType.Desktop,
            Status = LoginStatus.Enabled,
            CreatedOn = DateTimeOffset.UtcNow.AddHours(-24),
            ExpiredOn = DateTimeOffset.UtcNow.AddHours(-12)
        };

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode))
            .ReturnsAsync(playerLoginRecord);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(async ()
            => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, null).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_Only_No_Throws_Exception_When_Is_Null()
    {
        var playerCode = "player1";

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode))
            .ReturnsAsync((PlayerLoginRecordModel?)null);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(async ()
            => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, null).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_Only_No_Throws_When_Is_Valid()
    {
        var brandId = Guid.NewGuid();
        var playerCode = "player1";

        var playerLoginRecord = new PlayerLoginRecordModel
        {
            Id = Guid.NewGuid(),
            BrandId = brandId,
            PlayerCode = playerCode,
            PlatformType = PlatformType.Desktop,
            Status = LoginStatus.Enabled,
            CreatedOn = DateTimeOffset.UtcNow,
            ExpiredOn = DateTimeOffset.UtcNow.AddHours(12)
        };

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode))
            .ReturnsAsync(playerLoginRecord);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        Assert.DoesNotThrowAsync(async ()
            => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, null).ConfigureAwait(false));
    }
}

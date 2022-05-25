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
                x.GetLastAsync(playerCode, platformType, It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(playerLoginRecord);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        Assert.DoesNotThrowAsync(async ()
            => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platformType).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_And_Platform_Throws_Exception_When_Is_Null()
    {
        var playerCode = "player1";
        var platformType = PlatformType.Desktop;

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode, platformType, It.IsAny<DateTimeOffset>()))
            .ReturnsAsync((PlayerLoginRecordModel?)null);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(async ()
              => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platformType).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_With_PlayerCode_Only_No_Throws_Exception_When_Is_Null()
    {
        var playerCode = "player1";

        var playerLoginRecordServiceMock = new Mock<IRedisPlayerLoginRecordService>();

        _ = playerLoginRecordServiceMock.Setup(x =>
                x.GetLastAsync(playerCode, It.IsAny<DateTimeOffset>()))
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
                x.GetLastAsync(playerCode, It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(playerLoginRecord);

        var sut = new ExpirationValidator(playerLoginRecordServiceMock.Object);

        Assert.DoesNotThrowAsync(async ()
            => await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, null).ConfigureAwait(false));
    }
}

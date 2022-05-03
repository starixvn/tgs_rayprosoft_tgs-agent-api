#nullable enable

using System;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Tests.OperatorApi.UnitTests;

[TestFixture]
public class PlayerLoginRecordTests
{
    [Test]
    public void Validate_PlayerLoginRecord_Throw_Exception_When_LoginRecord_Is_Deauthorized()
    {
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var loginRecord = new LoginRecordInfo(
            Guid.NewGuid(),
            "player1",
            LoginStatus.Deauthorized,
            PlatformType.Desktop,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddHours(12));

        _ = fakePlayerRepository.GetPlayerLoginRecordAsync(Arg.Is(loginRecord.Id))
            .Returns(loginRecord);

        var command = new ValidatePlayerLoginRecordCommand(loginRecord.Id, loginRecord.PlayerCode);

        var sut = new ValidatePlayerLoginRecordCommandService(fakePlayerRepository);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(
            async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_Throw_Exception_When_LoginRecord_Is_Expired()
    {
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var loginRecord = new LoginRecordInfo(
            Guid.NewGuid(),
            "player1",
            LoginStatus.Enabled,
            PlatformType.Desktop,
            DateTimeOffset.UtcNow.AddDays(-2),
            DateTimeOffset.UtcNow.AddDays(-1));

        _ = fakePlayerRepository.GetPlayerLoginRecordAsync(Arg.Is(loginRecord.Id))
            .Returns(loginRecord);

        var command = new ValidatePlayerLoginRecordCommand(loginRecord.Id, loginRecord.PlayerCode);

        var sut = new ValidatePlayerLoginRecordCommandService(fakePlayerRepository);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(
            async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_Throw_Exception_When_LoginRecord_Is_Null()
    {
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var loginRecordId = Guid.NewGuid();
        var playerCode = "player1";

        _ = fakePlayerRepository.GetPlayerLoginRecordAsync(Arg.Is(loginRecordId))
            .Returns((LoginRecordInfo?)null);

        var command = new ValidatePlayerLoginRecordCommand(loginRecordId, playerCode);

        var sut = new ValidatePlayerLoginRecordCommandService(fakePlayerRepository);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(
            async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_Throw_Exception_When_PlayerCode_Is_Not_Match()
    {
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var loginRecord = new LoginRecordInfo(
            Guid.NewGuid(),
            "player1",
            LoginStatus.Enabled,
            PlatformType.Desktop,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddHours(12));

        _ = fakePlayerRepository.GetPlayerLoginRecordAsync(Arg.Is(loginRecord.Id))
            .Returns(loginRecord);

        var command = new ValidatePlayerLoginRecordCommand(loginRecord.Id, "player2");

        var sut = new ValidatePlayerLoginRecordCommandService(fakePlayerRepository);

        _ = Assert.ThrowsAsync<PlayerLoginExpiredException>(
              async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_PlayerLoginRecord_Throw_No_Exception_When_Is_Valid()
    {
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var loginRecord = new LoginRecordInfo(
            Guid.NewGuid(),
            "player1",
            LoginStatus.Enabled,
            PlatformType.Desktop,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddHours(12));

        _ = fakePlayerRepository.GetPlayerLoginRecordAsync(Arg.Is(loginRecord.Id))
              .Returns(loginRecord);

        var command = new ValidatePlayerLoginRecordCommand(loginRecord.Id, loginRecord.PlayerCode);

        var sut = new ValidatePlayerLoginRecordCommandService(fakePlayerRepository);

        Assert.DoesNotThrowAsync(async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }
}

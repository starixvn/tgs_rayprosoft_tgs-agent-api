using System;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Agent;

namespace Agent.UnitTests;

internal class AgentLoginRecordTests
{
    [Test]
    public void Validate_AgentLoginRecord_Throw_Exception_When_Is_Expired()
    {
        var fakeLoginRecordRepository = Substitute.For<IAgentLoginRecordRepository>();

        var loginRecordInfo = new AgentLoginRecordInfo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            LoginStatus.Enabled,
            DateTimeOffset.UtcNow.AddDays(-2),
            DateTimeOffset.UtcNow.AddDays(-1));

        _ = fakeLoginRecordRepository.GetAgentLoginRecordByIdAsync(Arg.Is(loginRecordInfo.LoginRecordId))
            .Returns(loginRecordInfo);

        var command = new ValidateAgentLoginRecordCommand(loginRecordInfo.LoginRecordId);

        var sut = new ValidateAgentLoginRecordCommandService(fakeLoginRecordRepository);

        _ = Assert.ThrowsAsync<Exception>(async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_AgentLoginRecord_Throw_Exception_When_Is_Null()
    {
        var fakeLoginRecordRepository = Substitute.For<IAgentLoginRecordRepository>();

        var loginRecordId = Guid.NewGuid();

        _ = fakeLoginRecordRepository.GetAgentLoginRecordByIdAsync(Arg.Is(loginRecordId))
            .Returns((AgentLoginRecordInfo?)null);

        var command = new ValidateAgentLoginRecordCommand(loginRecordId);

        var sut = new ValidateAgentLoginRecordCommandService(fakeLoginRecordRepository);

        _ = Assert.ThrowsAsync<Exception>(async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_AgentLoginRecord_When_Is_Valid()
    {
        var fakeLoginRecordRepository = Substitute.For<IAgentLoginRecordRepository>();

        var loginRecordInfo = new AgentLoginRecordInfo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            LoginStatus.Enabled,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddHours(12));

        _ = fakeLoginRecordRepository.GetAgentLoginRecordByIdAsync(Arg.Is(loginRecordInfo.LoginRecordId))
            .Returns(loginRecordInfo);

        var command = new ValidateAgentLoginRecordCommand(loginRecordInfo.LoginRecordId);

        var sut = new ValidateAgentLoginRecordCommandService(fakeLoginRecordRepository);

        Assert.DoesNotThrowAsync(async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }
}
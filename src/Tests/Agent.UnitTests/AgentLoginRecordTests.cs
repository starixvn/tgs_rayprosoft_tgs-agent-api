#nullable enable

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
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var loginRecordInfo = new AgentLoginRecordInfo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            LoginStatus.Enabled,
            DateTimeOffset.UtcNow.AddDays(-2),
            DateTimeOffset.UtcNow.AddDays(-1));

        _ = fakeAgentRepository.GetAgentLoginRecordByIdAsync(Arg.Is(loginRecordInfo.LoginRecordId))
            .Returns(loginRecordInfo);

        var command = new ValidateAgentLoginRecordCommand(loginRecordInfo.LoginRecordId);

        var sut = new ValidateAgentLoginRecordCommandService(fakeAgentRepository);

        _ = Assert.ThrowsAsync<Exception>(async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_AgentLoginRecord_Throw_Exception_When_Is_Null()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var loginRecordId = Guid.NewGuid();

        _ = fakeAgentRepository.GetAgentLoginRecordByIdAsync(Arg.Is(loginRecordId))
            .Returns((AgentLoginRecordInfo?)null);

        var command = new ValidateAgentLoginRecordCommand(loginRecordId);

        var sut = new ValidateAgentLoginRecordCommandService(fakeAgentRepository);

        _ = Assert.ThrowsAsync<Exception>(async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }

    [Test]
    public void Validate_AgentLoginRecord_When_Is_Valid()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var loginRecordInfo = new AgentLoginRecordInfo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            LoginStatus.Enabled,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddHours(12));

        _ = fakeAgentRepository.GetAgentLoginRecordByIdAsync(Arg.Is(loginRecordInfo.LoginRecordId))
            .Returns(loginRecordInfo);

        var command = new ValidateAgentLoginRecordCommand(loginRecordInfo.LoginRecordId);

        var sut = new ValidateAgentLoginRecordCommandService(fakeAgentRepository);

        Assert.DoesNotThrowAsync(async () => await sut.ExecuteAsync(command).ConfigureAwait(false));
    }
}

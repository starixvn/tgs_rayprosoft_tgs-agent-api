using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using TGS.Services.Agent.ApplicationServices.SubAgent;

namespace Agent.UnitTests;

[TestFixture]
public class SubAgentTests
{
    [Test]
    public async Task Reset_SubAgent_Password()
    {
        var agentId = Guid.NewGuid();
        var subAgentId = Guid.NewGuid();
        var password = "password";

        var command = new ResetSubAgentPasswordCommand(agentId, subAgentId, password);

        var fakeAgentClient = Substitute.For<IAgentClient>();

        var sut = new ResetSubAgentPasswordCommandService(fakeAgentClient);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAgentClient
            .Received(1)
            .ResetSubAgentPasswordAsync(Arg.Any<ResetSubAgentPasswordRequest>());
    }

    [Test]
    public async Task Set_SubAgent_Data()
    {
        var agentId = Guid.NewGuid();
        var subAgentId = Guid.NewGuid();
        var realName = "realName";
        var email = "email";
        var phone = "phone";

        var command = new SetSubAgentInfoCommand(agentId, subAgentId, realName, email, phone);

        var fakeAgentClient = Substitute.For<IAgentClient>();

        var sut = new SetSubAgentInfoCommandService(fakeAgentClient);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeAgentClient
            .Received(1)
            .SetSubAgentInfoAsync(Arg.Any<SetSubAgentInfoRequest>());
    }
}

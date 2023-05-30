using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.Agent.Client.Models.Enums;
using TGS.Services.Agent.ApplicationServices.SubAgent;

namespace Agent.UnitTests;

[TestFixture]
public class SubAgentTests
{
    [Test]
    public async Task Reset_SubAgent_Password()
    {
        var agentId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075c");
        var subAgentId = Guid.Parse("b9b3069f-d7ca-40d8-8996-a4df11986cd8");
        var password = "password";

        var command = new ResetSubAgentPasswordCommand(agentId, subAgentId, password);

        var fakeAgentClient = Substitute.For<IAgentClient>();

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(subAgentId))
            .Returns(new AgentInfoModel(
                subAgentId,
                "agent1",
                null,
                null,
                null,
                null,
                null,
                "nc97ra",
                ParentAgentId: agentId,
                DateTimeOffset.UtcNow,
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Agent,
                80M,
                0,
                0M));

        var sut = new ResetSubAgentPasswordCommandService(fakeAgentClient);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient
            .Received(1)
            .SetAgentPasswordAsync(new SetAgentPasswordRequest(command.SubAgentId, command.Password));
    }

    [Test]
    public async Task Set_SubAgent_Data()
    {
        var agentId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075c");
        var subAgentId = Guid.Parse("b9b3069f-d7ca-40d8-8996-a4df11986cd8");
        var realName = "realName";
        var email = "email";
        var phone = "phone";

        var command = new SetSubAgentInfoCommand(agentId, subAgentId, realName, email, phone);

        var fakeAgentClient = Substitute.For<IAgentClient>();

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(subAgentId))
            .Returns(new AgentInfoModel(
                subAgentId,
                "agent1",
                null,
                null,
                null,
                null,
                null,
                "nc97ra",
                ParentAgentId: agentId,
                DateTimeOffset.UtcNow,
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Agent,
                80M,
                0,
                0M));

        var sut = new SetSubAgentInfoCommandService(fakeAgentClient);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient
            .Received(1)
            .SetAgentInfoAsync(new SetAgentInfoRequest(
                command.SubAgentId,
                command.RealName,
                command.Email,
                command.Phone));
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.Agent.Client.Models.Enums;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.SubAgent;

namespace Agent.UnitTests;

[TestFixture]
public class SubAgentTests
{
    [Test]
    public async Task Get_SubAgent_Dividend_Range()
    {
        var fakeAgentClient = Substitute.For<IAgentClient>();

        var brandId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075d");
        var agentId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075c");
        var walletId = Guid.Parse("3980e08b-6bbc-4f0a-b027-90f837e0075d");
        var parentAgentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

        var childInfos = new ChildAgentInfoModel[]
        {
            new ChildAgentInfoModel
            {
                AgentId = Guid.Parse("30ae0dc5-f9a3-4441-95cc-12c6a8b92ffd"),
                AgentUsername = "agent2",
                DividendPercentage = 30M,
                ParentAgentId = subAgentId
            },
            new ChildAgentInfoModel
            {
                AgentId = Guid.Parse("0bda4cb0-cea0-4d7e-ab4e-17793074d801"),
                AgentUsername = "agent3",
                DividendPercentage = 20M,
                ParentAgentId = subAgentId
            },
        };

        _ = fakeAgentClient
            .GetChildAgentInfosAsync(new GetChildAgentInfosRequest(subAgentId))
            .Returns(childInfos.ToAsyncEnumerable());

        var parentAgent = new AgentInfoModel(parentAgentId, "parentUsername", null, null, null, null, null, "wg5g8h", null, DateTimeOffset.UtcNow.AddMonths(-10), AgentStatus.Enabled, AgentType.Cash, AgentLevelType.Chief, 90M, 0, 0M, brandId, walletId, walletId, false);

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(parentAgentId))
            .Returns(parentAgent);

        var sut = new GetSubAgentDividendRangeQueryService(fakeAgentClient);

        var actual = await sut.GetAsync(new GetSubAgentDividendRangeQuery(parentAgentId, subAgentId));

        Assert.AreEqual(30M, actual.Item1);
        Assert.AreEqual(90M, actual.Item2);
    }

    [Test]
    public async Task Reset_SubAgent_Password()
    {
        var brandId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075d");
        var agentId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075c");
        var walletId = Guid.Parse("3980e08b-6bbc-4f0a-b027-90f837e0075d");
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
                0M,
                brandId,
                walletId,
                walletId,
                false));

        var parent = new AgentInfoModel(
                agentId,
                "parent",
                null,
                null,
                null,
                null,
                null,
                "nc97ra",
                ParentAgentId: null,
                DateTimeOffset.UtcNow,
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Chief,
                80M,
                0,
                0M,
                brandId,
                walletId,
                walletId,
                false);

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(agentId))
            .Returns(parent);

        var sut = new ResetSubAgentPasswordCommandService(fakeAgentClient);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient
            .Received(1)
            .SetAgentPasswordAsync(new SetAgentPasswordRequest(command.SubAgentId, command.Password, parent.Username));

        _ = fakeAgentClient
            .Received(1)
            .AddAgentAuditLogAsync(new AddAgentAuditLogRequest(command.SubAgentId, AgentOperation.Password, parent.Username));
    }

    [Test]
    public async Task Set_SubAgent_Contact()
    {
        var brandId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075d");
        var agentId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075c");
        var walletId = Guid.Parse("3980e08b-6bbc-4f0a-b027-90f837e0075d");
        var subAgentId = Guid.Parse("b9b3069f-d7ca-40d8-8996-a4df11986cd8");
        var email = "newEmail";
        var phone = "newPhone";

        var command = new SetSubAgentContactCommand(agentId, subAgentId, email, phone);

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
                60M,
                0,
                0M,
                brandId,
                walletId,
                walletId,
                false));

        var parentAgent = new AgentInfoModel(
            agentId,
            "agent1",
            null,
            null,
            null,
            null,
            null,
            "uayygs",
            ParentAgentId: null,
            DateTimeOffset.UtcNow,
            AgentStatus.Enabled,
            AgentType.Cash,
            AgentLevelType.Chief,
            80M,
            0,
            0M,
            brandId,
            walletId,
            walletId,
            false);

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(agentId))
            .Returns(parentAgent);

        var sut = new SetSubAgentContactCommandService(fakeAgentClient);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient
            .Received(1)
            .SetAgentContactAsync(new SetAgentContactRequest(
                command.SubAgentId,
                command.Email,
                command.Phone));

        _ = fakeAgentClient
            .Received(1)
            .AddAgentAuditLogAsync(new AddAgentAuditLogRequest(
                command.SubAgentId,
                AgentOperation.Contact,
                parentAgent.Username));
    }

    [Test]
    public async Task Set_SubAgent_Dividend_Percentage_Will_Do_Nothing_When_Brand_AgentDividendType_Is_MultipleLayer()
    {
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeAgentClient = Substitute.For<IAgentClient>();

        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        _ = fakeBrandRepository.GetBrandInfoAsync(brandId)
            .Returns(new BrandInfo(
                brandId,
                "GPI",
                false,
                false,
                TGS.Core.Models.Enums.AgentDividendType.MultipleLayer,
                null,
                0));

        var sut = new SetSubAgentDividendPercentageCommandService(fakeBrandRepository, fakeAgentClient);

        var command = new SetSubAgentDividendPercentageCommand(
            brandId,
            Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31"),
            Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7"),
            68M);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient.Received(0)
            .SetAgentDividendPercentageAsync(new SetAgentDividendPercentageRequest(command.SubAgentId, command.DividendPercentage));
    }

    [Test]
    public async Task Set_SubAgent_Info()
    {
        var brandId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075d");
        var agentId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075c");
        var walletId = Guid.Parse("3980e08b-6bbc-4f0a-b027-90f837e0075d");
        var subAgentId = Guid.Parse("b9b3069f-d7ca-40d8-8996-a4df11986cd8");
        var realName = "realName";

        var command = new SetSubAgentInfoCommand(agentId, subAgentId, realName);

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
                60M,
                0,
                0M,
                brandId,
                walletId,
                walletId,
                false));

        var parentAgent = new AgentInfoModel(
            agentId,
            "agent1",
            null,
            null,
            null,
            null,
            null,
            "uayygs",
            ParentAgentId: null,
            DateTimeOffset.UtcNow,
            AgentStatus.Enabled,
            AgentType.Cash,
            AgentLevelType.Chief,
            80M,
            0,
            0M,
            brandId,
            walletId,
            walletId,
            false);

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(agentId))
            .Returns(parentAgent);

        var sut = new SetSubAgentInfoCommandService(fakeAgentClient);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient
            .Received(1)
            .SetAgentInfoAsync(new SetAgentInfoRequest(
                command.SubAgentId,
                command.RealName));

        _ = fakeAgentClient
            .Received(1)
            .AddAgentAuditLogAsync(new AddAgentAuditLogRequest(
                command.SubAgentId,
                AgentOperation.Info,
                parentAgent.Username));
    }

    [Test]
    public async Task SubAgent_Dividend_Percentage_Can_Not_Less_Than_Sub_SubAgents_Max_30_Or_More_Than_Parent_90()
    {
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeAgentClient = Substitute.For<IAgentClient>();

        var brandId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075d");
        var walletId = Guid.Parse("3980e08b-6bbc-4f0a-b027-90f837e0075d");
        var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        _ = fakeBrandRepository.GetBrandInfoAsync(brandId)
            .Returns(new BrandInfo(
                brandId,
                "GPI",
                false,
                false,
                TGS.Core.Models.Enums.AgentDividendType.OneLayer,
                null,
                0));

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(subAgentId))
            .Returns(new AgentInfoModel(
                Id: subAgentId,
                "childUsername",
                null,
                null,
                null,
                null,
                null,
                "bxdccs",
                agentId,
                DateTimeOffset.UtcNow.AddMonths(-2),
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Agent,
                50M,
                0,
                0M,
                brandId,
                walletId,
                walletId,
                false));

        _ = fakeAgentClient
            .GetChildAgentInfosAsync(new GetChildAgentInfosRequest(subAgentId))
            .Returns(
                new ChildAgentInfoModel[] {
                    new ChildAgentInfoModel() {
                        AgentId = brandId,
                        AgentUsername = "agent2",
                        DividendPercentage = 30,
                        ParentAgentId = subAgentId
                    },
                    new ChildAgentInfoModel() {
                            AgentId = brandId,
                            AgentUsername = "agent3",
                            DividendPercentage = 20,
                            ParentAgentId = subAgentId
                        }}.ToAsyncEnumerable());

        var parentAgent = new AgentInfoModel(
                Id: agentId,
                "parentUsername",
                null,
                null,
                null,
                null,
                null,
                "wg5g8h",
                null,
                DateTimeOffset.UtcNow.AddMonths(-10),
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Chief,
                90M,
                0,
                0M,
                brandId,
                walletId,
                walletId,
                false);

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(agentId))
            .Returns(parentAgent);

        var sut = new SetSubAgentDividendPercentageCommandService(fakeBrandRepository, fakeAgentClient);

        var command = new SetSubAgentDividendPercentageCommand(
            brandId,
            agentId,
            subAgentId,
            20M);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient.Received(0)
            .SetAgentDividendPercentageAsync(new SetAgentDividendPercentageRequest(command.SubAgentId, command.DividendPercentage));
    }

    [Test]
    public async Task SubAgent_Dividend_Percentage_Should_Between_Sub_SubAgent_And_Parent()
    {
        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeAgentClient = Substitute.For<IAgentClient>();

        var brandId = Guid.Parse("4980e08b-6bbc-4f0a-b027-90f837e0075d");
        var walletId = Guid.Parse("3980e08b-6bbc-4f0a-b027-90f837e0075d");
        var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        _ = fakeBrandRepository.GetBrandInfoAsync(brandId)
            .Returns(new BrandInfo(
                brandId,
                "GPI",
                false,
                false,
                TGS.Core.Models.Enums.AgentDividendType.OneLayer,
                null,
                0));

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(subAgentId))
            .Returns(new AgentInfoModel(
                Id: subAgentId,
                "childUsername",
                null,
                null,
                null,
                null,
                null,
                "bxdccs",
                agentId,
                DateTimeOffset.UtcNow.AddMonths(-2),
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Agent,
                50M,
                0,
                0M,
                brandId,
                walletId,
                walletId,
                false));

        _ = fakeAgentClient
            .GetChildAgentInfosAsync(new GetChildAgentInfosRequest(subAgentId))
            .Returns(
                new ChildAgentInfoModel[] {
                    new ChildAgentInfoModel() {
                        AgentId = brandId,
                        AgentUsername = "agent2",
                        DividendPercentage = 30,
                        ParentAgentId = subAgentId
                    },
                    new ChildAgentInfoModel() {
                            AgentId = brandId,
                            AgentUsername = "agent3",
                            DividendPercentage = 20,
                            ParentAgentId = subAgentId
                }}.ToAsyncEnumerable());

        var parentAgent = new AgentInfoModel(
                Id: agentId,
                "parentUsername",
                null,
                null,
                null,
                null,
                null,
                "wg5g8h",
                null,
                DateTimeOffset.UtcNow.AddMonths(-10),
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Chief,
                90M,
                0,
                0M,
                brandId,
                walletId,
                walletId,
                false);

        _ = fakeAgentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(agentId))
            .Returns(parentAgent);

        var sut = new SetSubAgentDividendPercentageCommandService(fakeBrandRepository, fakeAgentClient);

        var command = new SetSubAgentDividendPercentageCommand(
            brandId,
            agentId,
            subAgentId,
            90M);

        await sut.ExecuteAsync(command);

        _ = fakeAgentClient
            .Received(1)
            .SetAgentDividendPercentageAsync(new SetAgentDividendPercentageRequest(command.SubAgentId, command.DividendPercentage));

        _ = fakeAgentClient
            .Received(1)
            .AddAgentAuditLogAsync(new AddAgentAuditLogRequest(command.SubAgentId, AgentOperation.Info, parentAgent.Username));
    }
}

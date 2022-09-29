using System;
using System.Threading.Tasks;
using ExpectedObjects;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Agent;
using TGS.Services.WalletService;
using TGS.Services.Agent.ApplicationServices.Wallet;

namespace Agent.UnitTests;

[TestFixture]
public class WalletTests
{
    [Test]
    public async Task Agent_Can_Get_Balance_Info()
    {
        var moqAgentRepository = new Mock<IAgentRepository>();
        var moqAgentWalletRepository = new Mock<IAgentWalletRepository>();

        var sut = new GetBalanceQueryService(
            moqAgentRepository.Object,
            moqAgentWalletRepository.Object);

        var agentId = Guid.NewGuid();

        _ = moqAgentRepository.Setup(fake => fake.GetAgentByIdAsync(It.Is<Guid>(x => x == agentId)))
            .Returns(ValueTask.FromResult(new AgentInfo(
                agentId,
                "TestAgent",
                null,
                null,
                Guid.NewGuid(),
                "agent@test.com",
                null,
                null,
                "aaabbb",
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                null,
                DateTimeOffset.Now,
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Agent,
                0M,
                0)));

        var expected = new BalanceInfo(10000M).ToExpectedObject();

        _ = moqAgentWalletRepository
            .Setup(fake => fake.GetBalanceAsync(It.IsAny<Guid>(), It.Is<Guid>(x => x == agentId)))
            .Returns(ValueTask.FromResult(10000M));

        var actual = await sut.GetAsync(new GetBalanceQuery(agentId));

        expected.ShouldMatch(actual);
    }
}
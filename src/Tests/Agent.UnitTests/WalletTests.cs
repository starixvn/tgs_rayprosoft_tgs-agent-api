using System;
using System.Threading.Tasks;
using ExpectedObjects;
using Moq;
using NUnit.Framework;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Wallet;
using TGS.Services.WalletService;

namespace Agent.UnitTests
{
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

            moqAgentRepository.Setup(fake => fake.GetAgentByIdAsync(It.Is<Guid>(x => x == agentId)))
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
                    null,
                    1,
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    0M,
                    10M)));
            var expected = new BalanceInfo(10000M).ToExpectedObject();

            moqAgentWalletRepository
                .Setup(fake => fake.GetBalanceAsync(It.IsAny<Guid>(), It.Is<Guid>(x => x == agentId)))
                .Returns(ValueTask.FromResult(10000M));

            var actual = await sut.GetAsync(new GetBalanceQuery(agentId));

            expected.ShouldMatch(actual);
        }
    }
}
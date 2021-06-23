using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Agent.Services.Wallet;
using TGS.Services.Agent;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using ExpectedObjects;

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
                    Guid.NewGuid(),
                    "agent@test.com",
                    Array.Empty<byte>(),
                    Array.Empty<byte>())));
            var expected = new BalanceInfo(10000M).ToExpectedObject();

            moqAgentWalletRepository
                .Setup(fake => fake.GetBalanceAsync(It.IsAny<Guid>(), It.Is<Guid>(x => x == agentId)))
                .Returns(ValueTask.FromResult(10000M));

            var actual = await sut.GetAsync(new GetBalanceQuery(agentId));

            expected.ShouldMatch(actual);
        }
    }
}

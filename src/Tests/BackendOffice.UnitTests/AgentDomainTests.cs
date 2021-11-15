using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests
{
    public class AgentDomainTests
    {
        [Test]
        public async Task Add_Domain_To_A_Agent()
        {
            var moqAgentRepository = new Mock<IAgentRepository>();

            var sut = new AddAgentDomainCommandService(moqAgentRepository.Object);

            var command = new AddAgentDomainCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "test.com");

            await sut.ExecuteAsync(command);

            moqAgentRepository.Verify(
                fake => fake.AddDomainToAgentAsync(
                    It.Is<Guid>(x => x == command.AgentId),
                    It.Is<string>(x => x == command.DomainName)),
                Times.Exactly(1));
        }
    }
}

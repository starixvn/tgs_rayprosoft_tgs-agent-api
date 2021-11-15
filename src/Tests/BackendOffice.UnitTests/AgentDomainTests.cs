using System;
using System.Linq;
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
                "test.com");

            await sut.ExecuteAsync(command);

            moqAgentRepository.Verify(
                fake => fake.AddDomainToAgentAsync(
                    It.Is<Guid>(x => x == command.AgentId),
                    It.Is<string>(x => x == command.DomainName)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Domains_From_A_Agent()
        {
            var moqAgentRepository = new Mock<IAgentRepository>();

            var sut = new AgentDomainsQueryService(moqAgentRepository.Object);

            var query = new AgentDomainsQuery(Guid.NewGuid());

            moqAgentRepository
                .Setup(fake => fake.QueryDomainsFromAgentAsync(It.IsAny<Guid>()))
                .Returns(Array.Empty<string>().ToAsyncEnumerable());

            await sut.QueryAsync(query).ToArrayAsync();

            moqAgentRepository.Verify(
                fake => fake.QueryDomainsFromAgentAsync(It.Is<Guid>(x => x == query.AgentId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Remove_Domain_From_A_Agent()
        {
            var moqAgentRepository = new Mock<IAgentRepository>();

            var sut = new RemoveAgentDomainCommandService(moqAgentRepository.Object);

            var command = new RemoveAgentDomainCommand(
                Guid.NewGuid(),
                "test.com");

            await sut.ExecuteAsync(command);

            moqAgentRepository.Verify(
                fake => fake.RemoveDomainFromAgentAsync(
                    It.Is<Guid>(x => x == command.AgentId),
                    It.Is<string>(x => x == command.DomainName)),
                Times.Exactly(1));
        }
    }
}

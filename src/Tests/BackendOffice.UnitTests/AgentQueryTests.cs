using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Queries.AgentHandlers;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests;
public class AgentQueryTests
{
    [Test]
    public async Task Query_All_AgentInfo_By_Brand()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new AllAgentsByBrandQueryService(fakeAgentRepository);

        var query = new AllAgentsByBrandQuery(Guid.NewGuid());

        _ = await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeAgentRepository.Received(1)
            .GetAgentsAsync(Arg.Is(query.BrandId));
    }
}

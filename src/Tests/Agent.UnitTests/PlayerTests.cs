using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Paging;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Player;

namespace Agent.UnitTests
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public async Task Can_Get_Player_List()
        {
            var agentId = Guid.NewGuid();

            var fakeAgentRepository = Substitute.For<IAgentRepository>();

            var sut = new GetAgentPlayerInfosQueryService(fakeAgentRepository);

            var pagingInfo = new PagingInfo(1, 10, null, false);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                null,
                null,
                null,
                null,
                null,
                null,
                pagingInfo);

            await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            _ = fakeAgentRepository
                .Received(1)
                .QueryAgentPlayerInfosAsync(
                    query.AgentId,
                    query.Username,
                    query.BeginRegisteredOn,
                    query.EndRegisteredOn,
                    query.RegisterHost,
                    query.Status,
                    query.VipId,
                    Arg.Any<Paging<AgentPlayerInfo>>());
        }

        [Test]
        public async Task Can_Get_Player_List_Total_Count()
        {
            var agentId = Guid.NewGuid();

            var fakeAgentRepository = Substitute.For<IAgentRepository>();

            var sut = new CountAgentPlayerInfosQueryService(fakeAgentRepository);

            var pagingInfo = new PagingInfo(1, 10, null, false);

            var query = new GetAgentPlayerInfosQuery(
                agentId,
                null,
                null,
                null,
                null,
                null,
                null,
                pagingInfo);

            await sut.GetAsync(query).ConfigureAwait(false);

            _ = fakeAgentRepository
                .Received(1)
                .QueryAgentPlayerInfosTotalCountAsync(
                    query.AgentId,
                    query.Username,
                    query.BeginRegisteredOn,
                    query.EndRegisteredOn,
                    query.RegisterHost,
                    query.Status,
                    query.VipId);
        }

        [Test]
        public async Task Can_Get_VipLevels()
        {
            var brandId = Guid.NewGuid();

            var query = new GetPlayerVipLevelsQuery(brandId);

            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var sut = new GetPlayerVipLevelsQueryService(fakeBrandRepository);

            await sut.QueryAsync(query).ToArrayAsync().ConfigureAwait(false);

            _ = fakeBrandRepository
                .Received(1)
                .GetVipLevelsAsync(query.BrandId);
        }
    }
}

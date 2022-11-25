using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.BrandCertificate.Clients;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using Tgs.Paging;
using Tgs.PlayerService.Client;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Player;
using BrandInfo = Tgs.BrandCertificate.Clients.BrandInfo;
using PlayerStatus = Tgs.PlayerService.Client.PlayerStatus;
using VipLevelInfo = Tgs.BrandCertificate.Clients.VipLevelInfo;

namespace Agent.UnitTests;

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

    [Test]
    public async Task Can_Create_Player()
    {
        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var username = "username";
        var password = "password";

        var query = new CreatePlayerQuery(brandId, agentId, username, password);

        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new CreatePlayerQueryService(fakePlayerServiceClient,
            fakeAgentRepository,
            fakeBrandRepository,
            fakeBrandServiceClient);

        var agentInfo = new AgentInfo(
            agentId,
            username,
            RealName: username,
            Phone: "123",
            Email: "email@123",
            QQ: "QQ",
            Wechat: "wechat",
            ParentAgentId: null,
            DefaultWalletId: Guid.NewGuid(),
            InvitationCode: "code",
            PayPassword: Array.Empty<byte>(),
            PaySalt: Array.Empty<byte>(),
            Password: Array.Empty<byte>(),
            Salt: Array.Empty<byte>(),
            RegisteredOn: DateTimeOffset.UtcNow,
            Status: AgentStatus.Enabled,
            Type: AgentType.Cash,
            AgentLevelType: AgentLevelType.Agent,
            DividendPercentage: 10,
            PlayerNumber: 10,
            AccumulatedProfitSharing: 10);

        var brand = new BrandInfo { Id = brandId };

        var vipLevels = new[]
        {
            new VipLevelInfo(Guid.NewGuid(), 1)
        };

        var brandBetLimit = new BrandBetLimit { BetLimitId = 10 };
        var playerCode = "playerCode";

        fakePlayerServiceClient.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns((PlayerInfo)null);
        fakeAgentRepository.GetAgentByIdAsync(Arg.Any<Guid>()).Returns(agentInfo);
        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brand);
        fakeBrandServiceClient.GetVipLevelInfosAsync(Arg.Any<Guid>()).Returns(vipLevels.ToAsyncEnumerable());
        fakeBrandRepository.GetBrandDefaultBetLimitAsync(Arg.Any<Guid>(), default).Returns(brandBetLimit);
        fakePlayerServiceClient.CreatePlayerAsync(Arg.Any<CreatePlayerData>()).Returns(playerCode);

        await sut.GetAsync(query).ConfigureAwait(false);

        _ = fakePlayerServiceClient
            .Received(1)
            .CreatePlayerAsync(Arg.Any<CreatePlayerData>());

        _ = fakePlayerServiceClient
            .Received(1)
            .SetPlayerStatusAsync(Arg.Is(playerCode), Arg.Is(PlayerStatus.Enabled));
    }
}

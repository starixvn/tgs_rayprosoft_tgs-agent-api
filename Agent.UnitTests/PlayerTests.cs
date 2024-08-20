using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.BrandCertificate.Clients;
using Tgs.CashService.Client;
using TGS.Core.Models.Enums;
using Tgs.Paging;
using Tgs.Payment.Clients;
using Tgs.PlayerService.Client;
using Tgs.Service.PlayerBuckets;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Player;
using TGS.Services.Agent.ApplicationServices.Player.PlayerRegistrationConditions;
using TGS.Services.Agent.Exceptions;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.ViewModels;
using BrandInfo = Tgs.BrandCertificate.Clients.BrandInfo;
using PlayerInfo = Tgs.PlayerService.Client.PlayerInfo;
using PlayerStatus = Tgs.PlayerService.Client.PlayerStatus;

namespace Agent.UnitTests;

[TestFixture]
public class PlayerTests
{
    [Test]
    public async Task Can_Create_Player()
    {
        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var username = "username";
        var password = "password";
        var ipAddress = "127.0.0.1";
        var hostName = "localhost";

        var query = new CreatePlayerQuery(brandId, agentId, username, password, ipAddress, hostName);

        var fakeBrandRepository = Substitute.For<IBrandRepository>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();

        var sut = new CreatePlayerQueryService(fakePlayerServiceClient,
            fakeAgentRepository,
            fakeBrandRepository,
            fakeBrandServiceClient,
            fakeVipLevelServiceClient);

        var agentInfo = new AgentInfo(
            agentId,
            username,
            username,
            "123",
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

        var playerCode = "playerCode";

        fakePlayerServiceClient.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns((PlayerInfo?)null);
        fakeAgentRepository.GetAgentByIdAsync(Arg.Any<Guid>()).Returns(agentInfo);
        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brand);

        var vipLevelInfo = new VipLevelInfoViewModel(
            Guid.NewGuid(),
            1,
            brandId,
            0M,
            0M,
            0M,
            0M,
            false,
            0M,
            0M,
            0M,
            0M,
            0M,
            0M,
            0M,
            0M,
            0M,
            -1,
            -1m);
        _ = fakeVipLevelServiceClient.VipLevelGetAsync(
                Arg.Is(brandId),
                Arg.Is(1))
            .Returns(vipLevelInfo);

        fakeBrandRepository.GetBrandDefaultBetLimitIdAsync(Arg.Any<Guid>(), default).Returns(10);
        fakePlayerServiceClient.CreatePlayerAsync(Arg.Any<CreatePlayerData>()).Returns(playerCode);

        await sut.GetAsync(query).ConfigureAwait(false);

        _ = fakeVipLevelServiceClient.Received(1)
            .PlayerVipLevelRegisterAsync(
                Arg.Is(playerCode),
                Arg.Is(vipLevelInfo.Level),
                Arg.Is(PlayerVipLevelAutoState.Enable),
                cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePlayerServiceClient
            .Received(1)
            .CreatePlayerAsync(Arg.Any<CreatePlayerData>());

        _ = fakePlayerServiceClient
            .Received(1)
            .SetPlayerStatusAsync(Arg.Is(playerCode), Arg.Is(PlayerStatus.Enabled), Arg.Any<string>());
    }

    [Test]
    public async Task Can_Get_Player_Balance()
    {
        var agentId = Guid.Parse("71e90747-fdb5-4a67-b4df-8c75f2d1535f");

        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

        var sut = new GetPlayerBalanceQueryService(fakePlayerServiceClient, fakeCashServiceClient);

        var query = new GetPlayerBalanceQuery(
            agentId,
            "4uhyekcw88dy");

        var walletId = Guid.Parse("2b2b0a71-78cc-45b8-8f4b-08da9ad95d74");

        _ = fakePlayerServiceClient.GetPlayerPropertiesAsync(Arg.Is(query.PlayerCode), Arg.Any<CancellationToken>())
            .Returns(
                new Dictionary<string, string> { ["DefaultWalletId"] = walletId.ToString() }.ToImmutableDictionary());

        _ = fakeCashServiceClient.GetBalanceAsync(walletId, Arg.Any<CancellationToken>())
            .Returns(1000M);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.AreEqual(1000M, actual);
    }

    [Test]
    public async Task Can_Get_Player_List()
    {
        var agentId = Guid.NewGuid();

        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentPlayerInfosQueryService(fakeAgentRepository);

        var pagingInfo = new Paging<AgentPlayerInfo> { Page = 1, RowCount = 10, IsDesc = false };

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
                query.VipId);
    }

    [Test]
    public async Task Can_Get_Player_List_Total_Count()
    {
        var agentId = Guid.NewGuid();

        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentPlayerInfosQueryService(fakeAgentRepository);

        var pagingInfo = new Paging<AgentPlayerInfo> { Page = 1, RowCount = 10, IsDesc = false };

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
            .QueryAgentPlayerInfosAsync(
                query.AgentId,
                query.Username,
                query.BeginRegisteredOn,
                query.EndRegisteredOn,
                query.RegisterHost,
                query.Status,
                query.VipId);
    }

    [Test]
    public async Task Can_Get_Player_Payment_Overview()
    {
        var playerCode = "playerCode";

        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

        var sut = new GetPlayerPaymentOverviewQueryService(fakePaymentServiceClient);

        var query = new GetPlayerPaymentOverviewQuery(playerCode);

        _ = fakePaymentServiceClient.GetPlayerPaymentOverviewAsync(query.PlayerCode)
            .Returns(new PaymentOverview(100M, 0M, DateTime.Now, 0M, 0M, null, new DateOnly(2023, 4, 1), 100M, 0, 0M, 0));

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(100M, actual.TotalDepositAmount);
    }

    [Test]
    public async Task Can_Get_Player_Withdraw_Require_Amount()
    {
        var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();

        var sut = new GetPlayerWithdrawRequireAmountQueryService(fakePlayerBucketClient);

        var query = new GetPlayerWithdrawRequireAmountQuery("4uhyekcw88dy");

        _ = fakePlayerBucketClient
            .ReadBucketsAsync(Arg.Is(query.PlayerCode), Arg.Any<CancellationToken>())
            .Returns(new ReadBucketResult(168M, Guid.NewGuid()));

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.AreEqual(168M, actual);
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
    public void Return_Exception_When_Email_Is_Existed()
    {
        var brandId = Guid.NewGuid();
        var json = JsonDocument.Parse(
            "{\"username\":\"username\",\"areaCode\":\"+86\",\"phone\":\"0909090909\",\"email\":\"123@test.com\"}");

        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        _ = fakePlayerServiceClient.GetPlayerInfoByEmailAsync(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns(new PlayerInfo());

        var sut = new CheckEmailCondition(fakePlayerServiceClient);

        Assert.ThrowsAsync<PlayerEmailDuplicateException>(async () => await sut.ExecuteAsync(brandId, json, default));
    }

    [Test]
    public void Return_Exception_When_Phone_Is_Existed()
    {
        var brandId = Guid.NewGuid();
        var json = JsonDocument.Parse(
            "{\"username\":\"username\",\"areaCode\":\"+86\",\"phone\":\"0909090909\",\"email\":\"123@test.com\"}");

        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        _ = fakePlayerServiceClient.GetPlayerInfoByPhoneAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(new PlayerInfo());

        var sut = new CheckPhoneCondition(fakePlayerServiceClient);

        Assert.ThrowsAsync<PlayerPhoneDuplicateException>(async ()
            => await sut.ExecuteAsync(brandId, json, default).ConfigureAwait(false));
    }

    [Test]
    public void Return_Exception_When_Player_Is_Existed()
    {
        var brandId = Guid.NewGuid();
        var username = "username";
        var json = JsonDocument.Parse("{}");

        var query = new CheckRegistrationQuery(brandId, username, json);

        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeRegistrationConditionFuck = Substitute.For<Func<string, IRegistrationCondition?>>();

        var brandInfo = new BrandFrontendInfo(
            new Uri("http://123"),
            new Uri("http://123"),
            new Uri("http://123"),
            JsonDocument.Parse("[{\"type\":\"InitPhone\"}]"));

        _ = fakeBrandServiceClient.GetBrandFrontendInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        _ = fakePlayerServiceClient.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns(new PlayerInfo());

        var sut = new CheckRegistrationQueryService(fakeBrandServiceClient,
            fakePlayerServiceClient,
            fakeRegistrationConditionFuck);

        Assert.ThrowsAsync<PlayerExistAlreadyException>(async () => await sut.GetAsync(query).ConfigureAwait(false));
    }

    [Test]
    public async Task Return_True_When_Check_Registration_Conditions()
    {
        var brandId = Guid.NewGuid();
        var username = "username";
        var json = JsonDocument.Parse("{}");

        var query = new CheckRegistrationQuery(brandId, username, json);

        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeRegistrationConditionFuck = Substitute.For<Func<string, IRegistrationCondition?>>();

        var brandInfo = new BrandFrontendInfo(
            new Uri("http://123"),
            new Uri("http://123"),
            new Uri("http://123"),
            JsonDocument.Parse("[{\"type\":\"InitPhone\"}]"));

        _ = fakeBrandServiceClient.GetBrandFrontendInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        _ = fakePlayerServiceClient.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns((PlayerInfo?)null);

        var sut = new CheckRegistrationQueryService(fakeBrandServiceClient,
            fakePlayerServiceClient,
            fakeRegistrationConditionFuck);

        var response = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.IsTrue(response);
    }

    [Test]
    public async Task Success_For_Email_Not_Exists()
    {
        var brandId = Guid.NewGuid();
        var json = JsonDocument.Parse(
            "{\"username\":\"username\",\"areaCode\":\"+86\",\"phone\":\"0909090909\",\"email\":\"123@test.com\"}");

        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        _ = fakePlayerServiceClient.GetPlayerInfoByEmailAsync(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns((PlayerInfo?)null);

        var sut = new CheckEmailCondition(fakePlayerServiceClient);

        await sut.ExecuteAsync(brandId, json, default);
    }

    [Test]
    public async Task Success_For_Phone_Not_Exists()
    {
        var brandId = Guid.NewGuid();
        var json = JsonDocument.Parse(
            "{\"username\":\"username\",\"areaCode\":\"+86\",\"phone\":\"0909090909\",\"email\":\"123@test.com\"}");

        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        _ = fakePlayerServiceClient.GetPlayerInfoByPhoneAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns((PlayerInfo?)null);

        var sut = new CheckPhoneCondition(fakePlayerServiceClient);

        await sut.ExecuteAsync(brandId, json, default);
    }
}

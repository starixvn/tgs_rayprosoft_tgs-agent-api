using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.PlayerService.Client;
using Tgs.Report.Client;
using Tgs.Report.Client.Models;
using Tgs.Report.Client.Models.AgentWinLoss;
using Tgs.Report.Client.Models.Round;
using TGS.Core.Models.Enums;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.AgentWinLossReport;
using AgentPlayerSpecialDetailModel = Tgs.Report.Client.Models.AgentWinLoss.AgentPlayerSpecialDetailModel;
using AgentTotalDetailModel = Tgs.Report.Client.Models.AgentWinLoss.AgentTotalDetailModel;
using AgentWinLossModel = Tgs.Report.Client.Models.AgentWinLoss.AgentWinLossModel;
using CompanyTotalDetailModel = Tgs.Report.Client.Models.AgentWinLoss.CompanyTotalDetailModel;
using RoundModel = Tgs.Report.Client.Models.Round.RoundModel;
using RoundTotalModel = Tgs.Report.Client.Models.Round.RoundTotalModel;

namespace Agent.UnitTests;

[TestFixture]
public class AgentWinLossTests
{
    [Test]
    [AutoData]
    public async Task Can_Not_Get_Agent_Win_Loss_Breadcrumb(
        ChildAgentInfoModel[] childAgentInfoData,
        BreadcrumbModel[] breadcrumbData)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();

        var sut = new GetAgentWinLossBreadcrumbQueryService(
            reportClient,
            agentClient,
            playerClient);

        var query = new GetAgentWinLossBreadcrumbQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            MemberType = AccountType.Agent
        };

        _ = reportClient.GetAgentWinLossBreadcrumbAsync(Arg.Any<GetBreadcrumbRequest>()).Returns(breadcrumbData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());

        var response = await sut.GetAsync(query);

        _ = await reportClient.Received(1).GetAgentWinLossBreadcrumbAsync(Arg.Any<GetBreadcrumbRequest>());
        _ = agentClient.Received(1).GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>());
        _ = await playerClient.Received(0).GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>());

        Assert.IsTrue(!response.Any());
    }

    [Test]
    [AutoData]
    public async Task Export_Agent_Win_Loss_By_Agent_Username(
    ChildAgentInfoModel[] childAgentInfoData,
    PagedResponse<AgentWinLossModel> agentWinLossData,
    BrandInfo brandInfo,
    AgentInfoModel agentInfo)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var sut = new ExportAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new ExportAgentWinLossQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = childAgentInfoData.First().AgentUsername,
            MemberType = AccountType.Agent,
            DownLine = false
        };

        _ = reportClient.GetAgentWinLossAsync(Arg.Any<GetAgentWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = agentClient.GetAgentInfoByUsernameAsync(Arg.Any<GetAgentInfoByUsernameRequest>())
            .Returns(agentInfo);
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        _ = await sut.GetAsync(query);

        _ = agentClient.Received(1)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).ExportMasterAgentWinLossAsync(Arg.Any<ExportMasterAgentWinLossRequest>());
        _ = await reportClient.Received(1).ExportAgentWinLossAsync(Arg.Any<ExportAgentWinLossRequest>());
        _ = await reportClient.Received(0).ExportDownLineAgentWinLossAsync(Arg.Any<ExportDownLineAgentWinLossRequest>());
    }

    [Test]
    [AutoData]
    public async Task Export_Agent_Win_Loss_By_Logged_In_Agent(
    ChildAgentInfoModel[] childAgentInfoData,
    PagedResponse<AgentWinLossModel> agentWinLossData,
    BrandInfo brandInfo)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var sut = new ExportAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new ExportAgentWinLossQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = "",
            MemberType = AccountType.Agent,
            DownLine = true
        };

        _ = reportClient.GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        _ = await sut.GetAsync(query);

        _ = agentClient.Received(0)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).ExportMasterAgentWinLossAsync(Arg.Any<ExportMasterAgentWinLossRequest>());
        _ = await reportClient.Received(0).ExportAgentWinLossAsync(Arg.Any<ExportAgentWinLossRequest>());
        _ = await reportClient.Received(1).ExportDownLineAgentWinLossAsync(Arg.Any<ExportDownLineAgentWinLossRequest>());
    }

    [Test]
    [AutoData]
    public async Task Export_Agent_Win_Loss_By_Player_Username(
    ChildAgentInfoModel[] childAgentInfoData,
    PagedResponse<AgentWinLossModel> agentWinLossData,
    BrandInfo brandInfo,
    PlayerInfo playerInfo,
    Dictionary<string, string> playerProperties)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var sut = new ExportAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new ExportAgentWinLossQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = "playerUsername",
            MemberType = AccountType.Player,
            DownLine = false
        };

        playerProperties["AgentId"] = childAgentInfoData.First().AgentId.ToString();

        _ = reportClient.GetAgentPlayerWinLossAsync(Arg.Any<GetPlayerWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);
        _ = playerClient.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(playerInfo);
        _ = playerClient.GetPlayerPropertiesAsync(Arg.Any<string>()).Returns(playerProperties.ToImmutableDictionary());

        _ = await sut.GetAsync(query);

        _ = agentClient.Received(1)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).ExportMasterAgentWinLossAsync(Arg.Any<ExportMasterAgentWinLossRequest>());
        _ = await reportClient.Received(0).ExportAgentWinLossAsync(Arg.Any<ExportAgentWinLossRequest>());
        _ = await reportClient.Received(0).ExportDownLineAgentWinLossAsync(Arg.Any<ExportDownLineAgentWinLossRequest>());
        _ = await reportClient.Received(1).ExportPlayerWinLossAsync(Arg.Any<ExportPlayerWinLossRequest>());
    }

    [Test]
    [AutoData]
    public async Task Export_Agent_Win_Loss_Rounds(
        ChildAgentInfoModel[] childAgentInfoData,
        ExportWinLossRoundsQuery query,
        Dictionary<string, string> playerProperties)
    {
        var agentClient = Substitute.For<IAgentClient>();
        var client = Substitute.For<IReportClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();

        var sut = new ExportWinLossRoundsQueryService(
            client,
            playerClient,
            agentClient);

        playerProperties["AgentId"] = query.AgentId.ToString();
        _ = playerClient.GetPlayerPropertiesAsync(Arg.Any<string>()).Returns(playerProperties.ToImmutableDictionary());

        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());

        _ = await sut.GetAsync(query);

        _ = await client.Received(1).ExportRoundsAsync(Arg.Any<ExportRoundsRequest>());
    }

    [Test]
    [AutoData]
    public async Task Export_Down_Line_Agent_Win_Loss_By_Agent_Username(
        ChildAgentInfoModel[] childAgentInfoData,
        PagedResponse<AgentWinLossModel> agentWinLossData,
        BrandInfo brandInfo,
        AgentInfoModel agentInfo)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var sut = new ExportAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new ExportAgentWinLossQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = childAgentInfoData.First().AgentUsername,
            MemberType = AccountType.Agent,
            DownLine = true
        };

        _ = reportClient.GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = agentClient.GetAgentInfoByUsernameAsync(Arg.Any<GetAgentInfoByUsernameRequest>())
            .Returns(agentInfo);
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        _ = await sut.GetAsync(query);

        _ = agentClient.Received(1)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).ExportMasterAgentWinLossAsync(Arg.Any<ExportMasterAgentWinLossRequest>());
        _ = await reportClient.Received(0).ExportAgentWinLossAsync(Arg.Any<ExportAgentWinLossRequest>());
        _ = await reportClient.Received(1).ExportDownLineAgentWinLossAsync(Arg.Any<ExportDownLineAgentWinLossRequest>());
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Player_Special_Detail(
        GetAgentPlayerSpecialDetailQuery query,
        AgentPlayerSpecialDetailModel data,
        BrandInfo brandInfo)
    {
        var client = Substitute.For<IReportClient>();

        var sut = new GetAgentPlayerSpecialDetailQueryService(
            client);

        _ = client.GetAgentPlayerSpecialDetailAsync(Arg.Any<GetAgentPlayerSpecialDetailRequest>()).Returns(data);

        var response = await sut.GetAsync(query);

        Assert.AreEqual(data.Bonus, response.Bonus);
        Assert.AreEqual(data.BonusAdjust, response.BonusAdjust);
        Assert.AreEqual(data.ProductFee, response.ProductFee);
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Total_Detail(
        GetAgentTotalDetailQuery query,
        AgentTotalDetailModel[] data,
        BrandInfo brandInfo)
    {
        var client = Substitute.For<IReportClient>();
        var repository = Substitute.For<IBrandRepository>();

        var sut = new GetAgentTotalDetailQueryService(
            client,
            repository);

        client.GetAgentTotalDetailAsync(Arg.Any<GetAgentTotalDetailRequest>()).Returns(data);

        repository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        var response = await sut.GetAsync(query);

        await response.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(data[index].AgentSpecial, x.AgentSpecial);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Win_Loss_Breadcrumb(
        ChildAgentInfoModel[] childAgentInfoData)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();

        var sut = new GetAgentWinLossBreadcrumbQueryService(
            reportClient,
            agentClient,
            playerClient);

        var query = new GetAgentWinLossBreadcrumbQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            MemberType = AccountType.Agent
        };

        var breadcrumbData = childAgentInfoData.Select(x => new BreadcrumbModel
        {
            Id = x.AgentId.ToString(),
            Name = x.AgentUsername,
            Type = "Agent"
        }).ToArray();

        _ = reportClient.GetAgentWinLossBreadcrumbAsync(Arg.Any<GetBreadcrumbRequest>()).Returns(breadcrumbData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());

        var response = await sut.GetAsync(query);

        _ = await reportClient.Received(1).GetAgentWinLossBreadcrumbAsync(Arg.Any<GetBreadcrumbRequest>());
        _ = agentClient.Received(1).GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>());
        _ = await playerClient.Received(0).GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>());

        Assert.IsTrue(response.Any());

        await response.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(breadcrumbData[index].Id, x.Id);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Win_Loss_Breadcrumb_By_Player(
        ChildAgentInfoModel[] childAgentInfoData,
        PlayerInfo playerInfo)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();

        var sut = new GetAgentWinLossBreadcrumbQueryService(
            reportClient,
            agentClient,
            playerClient);

        var query = new GetAgentWinLossBreadcrumbQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = "playerUsername",
            MemberType = AccountType.Player
        };

        var breadcrumbData = new[]
        {
            new BreadcrumbModel { Id = childAgentInfoData.First().AgentId.ToString() }
        };

        _ = reportClient.GetAgentWinLossBreadcrumbAsync(Arg.Any<GetBreadcrumbRequest>()).Returns(breadcrumbData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = playerClient.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(playerInfo);
        var response = await sut.GetAsync(query);

        _ = await reportClient.Received(1).GetAgentWinLossBreadcrumbAsync(Arg.Any<GetBreadcrumbRequest>());
        _ = agentClient.Received(1).GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>());
        _ = await playerClient.Received(1).GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>());

        Assert.IsTrue(response.Any());

        await response.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(breadcrumbData[index].Id, x.Id);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Win_Loss_By_Agent_Username(
        ChildAgentInfoModel[] childAgentInfoData,
        PagedResponse<AgentWinLossModel> agentWinLossData,
        BrandInfo brandInfo)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var walletId = Guid.NewGuid();

        var sut = new GetAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new GetAgentWinLossQuery
        {
            BrandId = brandId,
            AgentId = agentId,
            Username = childAgentInfoData.First().AgentUsername,
            MemberType = AccountType.Agent,
            DownLine = false
        };

        var agentInfo = new AgentInfoModel(
            Id: childAgentInfoData.First().AgentId,
            Username: childAgentInfoData.First().AgentUsername,
            null,
            null,
            null,
            null,
            null,
            "",
            null,
            DateTimeOffset.UtcNow,
            Tgs.Agent.Client.Models.Enums.AgentStatus.Enabled,
            Tgs.Agent.Client.Models.Enums.AgentType.Cash,
            Tgs.Agent.Client.Models.Enums.AgentLevelType.Agent,
            0,
            0,
            0,
            brandId,
            walletId,
            walletId,
            false,
            true);

        _ = reportClient.GetAgentWinLossAsync(Arg.Any<GetAgentWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = agentClient.GetAgentInfoByUsernameAsync(Arg.Any<GetAgentInfoByUsernameRequest>())
            .Returns(agentInfo);
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        var response = await sut.GetAsync(query);

        _ = agentClient.Received(1)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).GetMasterAgentWinLossAsync(Arg.Any<GetMasterAgentWinLossRequest>());
        _ = await reportClient.Received(1).GetAgentWinLossAsync(Arg.Any<GetAgentWinLossRequest>());
        _ = await reportClient.Received(0).GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>());

        Assert.IsTrue(response.Items!.Any());

        await response.Items!.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(agentWinLossData.Items[index].Id, x.Id);
            Assert.AreEqual(agentWinLossData.Items[index].Turnover, x.Turnover);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Win_Loss_By_Logged_In_Agent(
        ChildAgentInfoModel[] childAgentInfoData,
        PagedResponse<AgentWinLossModel> agentWinLossData,
        BrandInfo brandInfo)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var sut = new GetAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new GetAgentWinLossQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = "",
            MemberType = AccountType.Agent,
            DownLine = true
        };

        _ = reportClient.GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        var response = await sut.GetAsync(query);

        _ = agentClient.Received(0)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).GetMasterAgentWinLossAsync(Arg.Any<GetMasterAgentWinLossRequest>());
        _ = await reportClient.Received(0).GetAgentWinLossAsync(Arg.Any<GetAgentWinLossRequest>());
        _ = await reportClient.Received(1).GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>());

        Assert.IsTrue(response.Items!.Any());

        await response.Items!.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(agentWinLossData.Items[index].Id, x.Id);
            Assert.AreEqual(agentWinLossData.Items[index].Turnover, x.Turnover);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Win_Loss_By_Player_Username(
        ChildAgentInfoModel[] childAgentInfoData,
        PagedResponse<AgentWinLossModel> agentWinLossData,
        BrandInfo brandInfo,
        PlayerInfo playerInfo,
        Dictionary<string, string> playerProperties)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var sut = new GetAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new GetAgentWinLossQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = "playerUsername",
            MemberType = AccountType.Player,
            DownLine = false
        };

        playerProperties["AgentId"] = childAgentInfoData.First().AgentId.ToString();

        _ = reportClient.GetAgentPlayerWinLossAsync(Arg.Any<GetPlayerWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);
        _ = playerClient.GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(playerInfo);
        _ = playerClient.GetPlayerPropertiesAsync(Arg.Any<string>()).Returns(playerProperties.ToImmutableDictionary());

        var response = await sut.GetAsync(query);

        _ = agentClient.Received(1)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).GetMasterAgentWinLossAsync(Arg.Any<GetMasterAgentWinLossRequest>());
        _ = await reportClient.Received(0).GetAgentWinLossAsync(Arg.Any<GetAgentWinLossRequest>());
        _ = await reportClient.Received(0).GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>());
        _ = await reportClient.Received(1).GetAgentPlayerWinLossAsync(Arg.Any<GetPlayerWinLossRequest>());

        Assert.IsTrue(response.Items!.Any());

        await response.Items!.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(agentWinLossData.Items[index].Id, x.Id);
            Assert.AreEqual(agentWinLossData.Items[index].Turnover, x.Turnover);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Company_Total_Detail(
        GetCompanyTotalDetailQuery query,
        CompanyTotalDetailModel[] data,
        BrandInfo brandInfo)
    {
        var client = Substitute.For<IReportClient>();
        var repository = Substitute.For<IBrandRepository>();

        var sut = new GetCompanyTotalDetailQueryService(
            client,
            repository);

        _ = client.GetCompanyTotalDetailAsync(Arg.Any<GetCompanyTotalDetailRequest>()).Returns(data);

        _ = repository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        var response = await sut.GetAsync(query);

        await response.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(data[index].CompanySpecial, x.CompanySpecial);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Down_Line_Agent_Win_Loss_By_Agent_Username(
        ChildAgentInfoModel[] childAgentInfoData,
        PagedResponse<AgentWinLossModel> agentWinLossData,
        BrandInfo brandInfo)
    {
        var reportClient = Substitute.For<IReportClient>();
        var agentClient = Substitute.For<IAgentClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var brandRepository = Substitute.For<IBrandRepository>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var walletId = Guid.NewGuid();

        var sut = new GetAgentWinLossQueryService(
            reportClient,
            brandRepository,
            agentClient,
            playerClient);

        var query = new GetAgentWinLossQuery
        {
            BrandId = Guid.NewGuid(),
            AgentId = Guid.NewGuid(),
            Username = childAgentInfoData.First().AgentUsername,
            MemberType = AccountType.Agent,
            DownLine = true
        };

        var agentInfo = new AgentInfoModel(
            Id: childAgentInfoData.First().AgentId,
            Username: childAgentInfoData.First().AgentUsername,
            null,
            null,
            null,
            null,
            null,
            "",
            null,
            DateTimeOffset.UtcNow,
            Tgs.Agent.Client.Models.Enums.AgentStatus.Enabled,
            Tgs.Agent.Client.Models.Enums.AgentType.Cash,
            Tgs.Agent.Client.Models.Enums.AgentLevelType.Agent,
            0,
            0,
            0,
            brandId,
            walletId,
            walletId,
            false,
            true);

        _ = reportClient.GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>()).Returns(agentWinLossData);
        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());
        _ = agentClient.GetAgentInfoByUsernameAsync(Arg.Any<GetAgentInfoByUsernameRequest>())
            .Returns(agentInfo);
        _ = brandRepository.GetBrandInfoAsync(Arg.Any<Guid>()).Returns(brandInfo);

        var response = await sut.GetAsync(query);

        _ = agentClient.Received(1)
            .GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>(), Arg.Any<CancellationToken>());

        _ = await reportClient.Received(0).GetMasterAgentWinLossAsync(Arg.Any<GetMasterAgentWinLossRequest>());
        _ = await reportClient.Received(0).GetAgentWinLossAsync(Arg.Any<GetAgentWinLossRequest>());
        _ = await reportClient.Received(1).GetDownLineAgentWinLossAsync(Arg.Any<GetDownLineAgentWinLossRequest>());

        Assert.IsTrue(response.Items!.Any());

        await response.Items!.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(agentWinLossData.Items[index].Id, x.Id);
            Assert.AreEqual(agentWinLossData.Items[index].Turnover, x.Turnover);
        });
    }

    [Test]
    [AutoData]
    public async Task Get_Win_Loss_Rounds(
        ChildAgentInfoModel[] childAgentInfoData,
        GetWinLossRoundsQuery query,
        PagedResponse<RoundModel> data,
        RoundTotalModel roundTotal,
        Dictionary<string, string> playerProperties)
    {
        var client = Substitute.For<IReportClient>();
        var playerClient = Substitute.For<IPlayerServiceClient>();
        var agentClient = Substitute.For<IAgentClient>();

        var sut = new GetWinLossRoundsQueryService(
            client,
            playerClient,
            agentClient);

        _ = client.GetRoundsAsync(Arg.Any<GetRoundsRequest>()).Returns(data);

        _ = client.GetRoundsTotalAsync(Arg.Any<GetRoundsTotalRequest>()).Returns(roundTotal);

        _ = agentClient.GetChildAgentInfosAsync(Arg.Any<GetChildAgentInfosRequest>())
            .Returns(childAgentInfoData.ToAsyncEnumerable());

        playerProperties["AgentId"] = childAgentInfoData.First().AgentId.ToString();
        _ = playerClient.GetPlayerPropertiesAsync(Arg.Any<string>()).Returns(playerProperties.ToImmutableDictionary());

        var response = await sut.GetAsync(query);

        await response.Items.ToAsyncEnumerable().ForEachAsync((x, index) =>
        {
            Assert.AreEqual(data.Items[index].SettleAmount, x.SettleAmount);
            Assert.AreEqual(data.Items[index].WinLoss, x.WinLoss);
            Assert.AreEqual(data.Items[index].BetAmount, x.BetAmount);
            Assert.AreEqual(data.Items[index].Turnover, x.Turnover);
        });

        Assert.AreEqual(roundTotal.BetAmount, response.Total.BetAmount);
    }
}

using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using Tgs.MarketingReport.Client;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
using Tgs.ServiceUtility;

namespace TGS.Tests.BackendOffice.UnitTests;

internal class MarketingReportTests
{
    [Test]
    public async Task Can_Get_Marketing_Report_By_Agent()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeReportClient = Substitute.For<IMarketingReportClient>();

        var paging = new PageableRequest
        {
            Page = 1,
            Rows = 10,
            ShowAll = false
        };

        var agentInfo = new AgentInfo(
            Guid.NewGuid(),
            "Agent1",
            AgentStatus.Enabled);

        var query = new GetMarketingReportAggregateInfosByAgentQuery(
            Guid.NewGuid(),
            agentInfo.UserName,
            DateOnly.Parse("2022-06-01"),
            DateOnly.Parse("2022-06-10"),
            paging);

        _ = fakeAgentRepository.GetAgentAsync(
                 Arg.Is(query.BrandId),
                 Arg.Is(query.AgentUserName))
             .Returns(agentInfo);

        var sut = new GetMarketingReportAggregateInfosByAgentQueryService(
            fakeAgentRepository,
            fakeReportClient);

        _ = await sut.GetAsync(query).ConfigureAwait(false);

        _ = fakeAgentRepository
            .Received(1)
            .GetAgentAsync(
                Arg.Is(query.BrandId),
                Arg.Is(query.AgentUserName));

        _ = fakeReportClient
            .Received(1)
            .GetDailyAggregateInfosAsync(
                Arg.Is(query.BrandId),
                Arg.Is(agentInfo.AgentId),
                Arg.Is(query.StartDate),
                Arg.Is(query.EndDate));
    }

    [Test]
    public async Task Can_Get_Marketing_Report_By_Brand()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeReportClient = Substitute.For<IMarketingReportClient>();
        var fakeAgentPlayerDepositQueryService =
            Substitute.For<IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics>>();

        var paging = new PageableRequest
        {
            Page = 1,
            Rows = 10,
            ShowAll = false
        };

        var query = new GetMarketingReportAggregateInfosQuery(
            Guid.NewGuid(),
            DateOnly.Parse("2022-06-01"),
            DateOnly.Parse("2022-06-10"),
            paging);

        var sut = new GetMarketingReportAggregateInfosQueryService(
            fakeAgentPlayerDepositQueryService,
            fakeAgentRepository,
            fakeReportClient);

        _ = await sut.GetAsync(query).ConfigureAwait(false);

        _ = fakeAgentRepository
            .Received(1)
            .GetAgentsAsync(Arg.Is(query.BrandId));

        _ = fakeAgentPlayerDepositQueryService
            .Received(1)
            .QueryAsync(Arg.Is(new AgentPlayerDepositQuery(query.BrandId)));

        _ = fakeReportClient
            .Received(1)
            .GetDailyAggregateInfosAsync(
                Arg.Is(query.BrandId),
                Arg.Is(query.StartDate),
                Arg.Is(query.EndDate));
    }
}

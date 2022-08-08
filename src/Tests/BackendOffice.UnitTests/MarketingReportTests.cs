using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.MarketingReport.Client;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.AgentHandlers;
using TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

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
    public async Task Get_MarketingReport_By_Brand_Sort_PropertyName()
    {
        var fakeReportClient = Substitute.For<IMarketingReportClient>();
        var fakeAgentPlayerDepositQueryService = Substitute.For<IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics>>();
        var fakeAgentQueryService = Substitute.For<IQueryService<AllAgentsByBrandQuery, AgentInfo>>();

        var paging = new PageableRequest
        {
            Page = 1,
            Rows = 10,
            SortBy = "reportDate",
            ShowAll = false
        };

        var query = new GetMarketingReportAggregateInfosQuery(
            Guid.NewGuid(),
            DateOnly.Parse("2022-06-01"),
            DateOnly.Parse("2022-06-10"),
            paging);

        _ = fakeAgentQueryService
            .QueryAsync(Arg.Is(new AllAgentsByBrandQuery(query.BrandId)))
            .Returns(new[] {
                new AgentInfo(Guid.NewGuid(), "Agent1", AgentStatus.Enabled)
            }.ToAsyncEnumerable());

        var sut = new GetMarketingReportAggregateInfosQueryService(
            fakeAgentPlayerDepositQueryService,
            fakeAgentQueryService,
            fakeReportClient);

        _ = await sut.GetAsync(query).ConfigureAwait(false);

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

    [Test]
    public async Task Can_Get_Marketing_Report_By_Brand()
    {
        var fakeReportClient = Substitute.For<IMarketingReportClient>();
        var fakeAgentPlayerDepositQueryService = Substitute.For<IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics>>();
        var fakeAgentQueryService = Substitute.For<IQueryService<AllAgentsByBrandQuery, AgentInfo>>();

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

        _ = fakeAgentQueryService
            .QueryAsync(Arg.Is(new AllAgentsByBrandQuery(query.BrandId)))
            .Returns(new[] {
                new AgentInfo(Guid.NewGuid(), "Agent1", AgentStatus.Enabled)
            }.ToAsyncEnumerable());

        var sut = new GetMarketingReportAggregateInfosQueryService(
            fakeAgentPlayerDepositQueryService,
            fakeAgentQueryService,
            fakeReportClient);

        _ = await sut.GetAsync(query).ConfigureAwait(false);

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

using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.MarketingReport.Client;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Marketing;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
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
    public async Task Can_Get_Marketing_Report_By_Brand()
    {
        var fakeReportClient = Substitute.For<IMarketingReportClient>();
        var fakeAgentPlayerDepositQueryService = Substitute.For<IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics>>();
        var fakeAgentQueryService = Substitute.For<IQueryService<AllAgentsByBrandQuery, AgentInfo>>();
        var fakePlayerPaymentDetailQueryService = Substitute.For<IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem>>();
        var fakeDailMarketingRegisterQueryService = Substitute.For<IQueryService<GetDailyMarketingRegisterDetailQuery, PlayerDailyRegisterItem>>();

        var sut = new GetMarketingReportAggregateInfosQueryService(
            fakeAgentPlayerDepositQueryService,
            fakeAgentQueryService,
            fakePlayerPaymentDetailQueryService,
            fakeDailMarketingRegisterQueryService,
            fakeReportClient);

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
    public async Task Get_MarketingReport_By_Brand_Sort_PropertyName()
    {
        var fakeReportClient = Substitute.For<IMarketingReportClient>();
        var fakeAgentPlayerDepositQueryService = Substitute.For<IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics>>();
        var fakeAgentQueryService = Substitute.For<IQueryService<AllAgentsByBrandQuery, AgentInfo>>();
        var fakePlayerPaymentDetailQueryService = Substitute.For<IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem>>();
        var fakeDailMarketingRegisterQueryService = Substitute.For<IQueryService<GetDailyMarketingRegisterDetailQuery, PlayerDailyRegisterItem>>();

        var sut = new GetMarketingReportAggregateInfosQueryService(
            fakeAgentPlayerDepositQueryService,
            fakeAgentQueryService,
            fakePlayerPaymentDetailQueryService,
            fakeDailMarketingRegisterQueryService,
            fakeReportClient);

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
    public async Task Get_Player_Register_Detail()
    {
        var fakeReportClient = Substitute.For<IReportClient>();
        var fakeQueryAgentService = Substitute.For<IQueryService<AllAgentsByBrandQuery, AgentInfo>>();
        var fakeQueryPlayerDailyRegisterService = Substitute.For<IQueryService<GetDailyMarketingRegisterDetailQuery, PlayerDailyRegisterItem>>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var sut = new GetMarketingRegisterDetailQuery(
            fakeReportClient,
            fakeQueryAgentService,
            fakeQueryPlayerDailyRegisterService,
            fakeSystemClock);

        var query = new GetMarketingRegisterDetailReportRequest
        {
            BrandId = Guid.NewGuid(),
            Date = DateTime.Now.AddDays(-1D),
            Page = 1,
            Rows = 10
        };

        _ = fakeReportClient
            .GetMarketingRegisterDetailsAsync(Arg.Is<GetMarketingRegisterDetailRequest>(
                request => request.BrandId == query.BrandId
                    && request.Date == new DateTimeOffset(
                        query.Date.ToOffset(TimeSpan.FromHours(8D)).Date,
                        TimeSpan.FromHours(8D))))
            .Returns(new Tgs.Report.Client.Models.PagedResponse<MarketingRegisterDetailModel>
            {
                Valid = true,
                TotalCount = 0,
                Items = Array.Empty<MarketingRegisterDetailModel>()
            });

        var actual = await sut.GetAsync(query);

        Assert.IsNotNull(actual);
    }

    [Test]
    public async Task Get_ReadTime_PlayerRegister_Detail()
    {
        var fakeReportClient = Substitute.For<IReportClient>();
        var fakeQueryAgentService = Substitute.For<IQueryService<AllAgentsByBrandQuery, AgentInfo>>();
        var fakeQueryPlayerDailyRegisterService = Substitute.For<IQueryService<GetDailyMarketingRegisterDetailQuery, PlayerDailyRegisterItem>>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var sut = new GetMarketingRegisterDetailQuery(
            fakeReportClient,
            fakeQueryAgentService,
            fakeQueryPlayerDailyRegisterService,
            fakeSystemClock);

        var query = new GetMarketingRegisterDetailReportRequest
        {
            BrandId = Guid.NewGuid(),
            AgentUsername = "Agent1",
            Date = DateTime.Now,
            Page = 1,
            Rows = 10
        };

        _ = fakeSystemClock.UtcNow.Returns(DateTimeOffset.UtcNow);

        _ = fakeQueryAgentService.QueryAsync(Arg.Any<AllAgentsByBrandQuery>())
            .Returns(new[] {
                new AgentInfo(Guid.NewGuid(), "Agent1", AgentStatus.Enabled)
            }.ToAsyncEnumerable());

        var actual = await sut.GetAsync(query);

        _ = fakeQueryPlayerDailyRegisterService.Received(1)
            .QueryAsync(Arg.Is<GetDailyMarketingRegisterDetailQuery>(q => q.BrandId == q.BrandId
                && q.QueryDate == DateOnly.FromDateTime(query.Date.ToOffset(TimeSpan.FromHours(8D)).DateTime)));

        Assert.IsNotNull(actual);
    }

    [Test]
    public async Task Get_RealTime_PlayerRegister_Statistics()
    {
        var fakeMarketingReportRepository = Substitute.For<IMarketingReportRepository>();

        var sut = new GetDailyMarketingRegisterDetailQueryService(fakeMarketingReportRepository);

        var query = new GetDailyMarketingRegisterDetailQuery(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now));

        await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeMarketingReportRepository.Received(1)
            .QueryRealTimePlayerDailyRegisterStatisticsAsync(Arg.Is(query.BrandId), Arg.Is(query.QueryDate));
    }
}

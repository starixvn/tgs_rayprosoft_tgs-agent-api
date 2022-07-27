using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests;

internal class MarketingReportFirstDepositDetailQueryTests
{
    [Test]
    public async Task Can_Get_Marketing_Report_First_Deposit_Detail()
    {
        var fakeQueryService =
            Substitute.For<IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem>>();

        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var paging = new PageableRequest
        {
            Page = 1,
            Rows = 10,
            ShowAll = false
        };

        var query = new GetMarketingFirstDepositDetailQuery(
            Guid.NewGuid(),
            DateOnly.Parse("2022-06-01"),
            "Agent123",
            paging);

        var sut = new GetMarketingFirstDepositDetailQueryService(
            fakeQueryService,
            fakeAgentRepository);

        _ = await sut.GetAsync(query).ConfigureAwait(false);

        _ = fakeAgentRepository
            .Received(1)
            .GetAgentAsync(Arg.Is(query.BrandId), Arg.Is(query.AgentUsername));

        _ = fakeQueryService
            .Received(1)
            .QueryAsync(Arg.Is(new PlayerPaymentStatisticsDetailQuery(query.BrandId, query.Date)));
    }
}

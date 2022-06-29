using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.MarketingReport.Client;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

internal class GetMarketingReportAggregateInfosQueryService
    : IGetService<GetMarketingReportAggregateInfosQuery, MarketingReportResponse>
{
    private static readonly TimeSpan _Offset = TimeSpan.FromHours(8D);

    private readonly IAgentRepository _agentRepository;
    private readonly IMarketingReportClient _reportClient;

    public GetMarketingReportAggregateInfosQueryService(
        IAgentRepository agentRepository,
        IMarketingReportClient reportClient)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
    }

    public async ValueTask<MarketingReportResponse> GetAsync(
        GetMarketingReportAggregateInfosQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = query.StartDate;
        var endDate = query.EndDate;

        var agents = await _agentRepository
            .GetAgentsAsync(query.BrandId)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var dates = Enumerable.Range(0, endDate.DayNumber - startDate.DayNumber)
            .Select(x => startDate.AddDays(x))
            .ToArray();

        var aggregateInfos = await _reportClient
            .GetDailyAggregateInfosAsync(query.BrandId, startDate, endDate, cancellationToken)
            .ToArrayAsync(cancellationToken);

        var aggregates =
            (from date in dates
             from agent in agents
             join info in aggregateInfos on new { Date = date, agent.AgentId }
                equals new { Date = info.ReportDate, info.AgentId } into gj
             from items in gj.DefaultIfEmpty()
             select new MarketingReportModel
             {
                 ReportDate = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, _Offset),
                 AgentId = agent.AgentId,
                 AgentUsername = agent.UserName,
                 RegisterCount = items?.RegisterCount ?? 0,
                 FirstDepositCount = items?.FirstDepositCount ?? 0,
                 GamingCount = items?.GamingCount ?? 0
             }).ToArray();

        var pagingResponse = aggregates.AsQueryable().CreatePagedResponse(query.Paging);

        var total = new MarketingReportTotalModel
        {
            RegisterCount = aggregates.Sum(a => a.RegisterCount),
            FirstDepositCount = aggregates.Sum(a => a.FirstDepositCount),
            GamingCount = aggregates.Sum(a => a.GamingCount)
        };

        return new MarketingReportResponse
        {
            Valid = true,
            TotalCount = pagingResponse.TotalCount,
            Items = pagingResponse.Items,
            Total = total
        };
    }
}

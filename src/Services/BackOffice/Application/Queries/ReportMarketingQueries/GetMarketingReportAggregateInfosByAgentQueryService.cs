using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using Tgs.MarketingReport.Client;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

internal class GetMarketingReportAggregateInfosByAgentQueryService
    : IGetService<GetMarketingReportAggregateInfosByAgentQuery, MarketingReportResponse>
{
    private static readonly TimeSpan _Offset = TimeSpan.FromHours(8D);

    private readonly IMarketingReportClient _reportClient;
    private readonly ITgsRepository _tgsRepository;

    public GetMarketingReportAggregateInfosByAgentQueryService(
        ITgsRepository tgsRepository,
        IMarketingReportClient reportClient)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
    }

    public async ValueTask<MarketingReportResponse> GetAsync(
        GetMarketingReportAggregateInfosByAgentQuery query,
        CancellationToken cancellationToken = default)
    {
        var startDate = query.StartDate;
        var endDate = query.EndDate;

        var agent = await _tgsRepository.Agents
            .Where(x => x.BrandId == query.BrandId && x.Username == query.AgentUserName)
            .Select(a => new { AgentId = a.Id, AgentUserName = a.Username })
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (agent == null)
        {
            return new MarketingReportResponse
            {
                Valid = true,
                Items = Array.Empty<MarketingReportModel>()
            };
        }

        var dates = Enumerable.Range(0, endDate.DayNumber - startDate.DayNumber)
            .Select(x => startDate.AddDays(x))
            .ToArray();

        var aggregateInfos = await _reportClient
            .GetDailyAggregateInfosAsync(query.BrandId, agent.AgentId, startDate, endDate, cancellationToken)
            .ToArrayAsync(cancellationToken);

        var aggregates =
            (from date in dates
             join info in aggregateInfos on date equals info.ReportDate into gj
             from items in gj.DefaultIfEmpty()
             select new MarketingReportModel
             {
                 ReportDate = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, _Offset),
                 AgentId = agent.AgentId,
                 AgentUsername = agent.AgentUserName,
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

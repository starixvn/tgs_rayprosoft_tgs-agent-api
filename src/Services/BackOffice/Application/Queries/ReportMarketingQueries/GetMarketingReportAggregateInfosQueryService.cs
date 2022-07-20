using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.MarketingReport.Client;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

internal class GetMarketingReportAggregateInfosQueryService
    : IGetService<GetMarketingReportAggregateInfosQuery, MarketingReportResponse>
{
    private static readonly TimeSpan _Offset = TimeSpan.FromHours(8D);

    private readonly IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics> _agentPlayerDepositQueryService;
    private readonly IAgentRepository _agentRepository;
    private readonly IMarketingReportClient _reportClient;

    public GetMarketingReportAggregateInfosQueryService(
        IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics> agentPlayerDepositQueryService,
        IAgentRepository agentRepository,
        IMarketingReportClient reportClient)
    {
        _agentPlayerDepositQueryService = agentPlayerDepositQueryService ?? throw new ArgumentNullException(nameof(agentPlayerDepositQueryService));
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

        var data = _agentPlayerDepositQueryService.QueryAsync(
            new AgentPlayerDepositQuery(query.BrandId),
            cancellationToken);

        var depositStatistics = await data
            .Where(s => s.ReportDate >= startDate && s.ReportDate < endDate)
            .ToArrayAsync(cancellationToken);

        var depositJoin =
            from date in dates
            from agent in agents
            join info in depositStatistics on new { Date = date, agent.AgentId }
                equals new { Date = info.ReportDate, info.AgentId } into gj
            from item in gj.DefaultIfEmpty()
            select new
            {
                Date = date,
                AgentId = agent.AgentId,
                AgentUername = agent.UserName,
                FirstDepositCount = item?.FirstDepositPlayers ?? 0
            };

        var aggregates =
            (from depositInfo in depositJoin
             join info in aggregateInfos on new { Date = depositInfo.Date, depositInfo.AgentId }
                equals new { Date = info.ReportDate, info.AgentId } into gj
             from items in gj.DefaultIfEmpty()
             let date = depositInfo.Date
             select new MarketingReportModel
             {
                 ReportDate = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, _Offset),
                 AgentId = depositInfo.AgentId,
                 AgentUsername = depositInfo.AgentUername,
                 RegisterCount = items?.RegisterCount ?? 0,
                 FirstDepositCount = depositInfo?.FirstDepositCount ?? 0,
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

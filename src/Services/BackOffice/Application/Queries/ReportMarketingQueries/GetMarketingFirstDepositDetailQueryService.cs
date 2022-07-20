using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Marketing;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
using Tgs.ServiceUtility;
using TGS.Paging;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

public class GetMarketingFirstDepositDetailQueryService
    : IGetService<GetMarketingFirstDepositDetailQuery, PagedResponse<MarketingFirstDepositDetailReportModel>>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem> _queryService;

    public GetMarketingFirstDepositDetailQueryService(
         IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem> queryService,
         IAgentRepository agentRepository)
    {
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
    }

    public async ValueTask<PagedResponse<MarketingFirstDepositDetailReportModel>> GetAsync(
        GetMarketingFirstDepositDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository
            .GetAgentAsync(query.BrandId, query.AgentUsername, cancellationToken)
            .ConfigureAwait(false);

        var detailQuery = new PlayerPaymentStatisticsDetailQuery(query.BrandId, query.Date);
        var statisticsData = await _queryService.QueryAsync(detailQuery, cancellationToken)
            .Where(item => item.AgentId == agent!.AgentId
                           && item.IsFirstDeposit == true)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var dataQuery = statisticsData.AsQueryable();

        var paging = new Paging<PlayerDailyPaymentItem>();

        if (!string.IsNullOrWhiteSpace(query.Paging.SortBy))
            paging.SortByPropertyName(query.Paging.SortBy, query.Paging.IsDesc);

        paging.Page = query.Paging.Page;
        paging.RowCount = query.Paging.Rows;

        var totalCount = statisticsData.Length;

        dataQuery = dataQuery.ApplyPaging(paging);

        return new PagedResponse<MarketingFirstDepositDetailReportModel>
        {
            TotalCount = totalCount,
            Items = dataQuery
                .Select(item => new MarketingFirstDepositDetailReportModel
                {
                    Username = item.PlayerName,
                    FirstDepositOn = item.FirstDepositOn,
                    FirstDepositAmount = item.FirstDepositAmount,
                    TotalDepositAmount = item.TotalDepositAmount,
                    TotalWithdrawAmount = item.TotalWithdrawAmount,
                    DepositSubtractWithdraw = item.TotalDepositAmount - item.TotalWithdrawAmount,
                    IpAddress = item.RegisterIp
                })
                .ToArray()
        };
    }
}

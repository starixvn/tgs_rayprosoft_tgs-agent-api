using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Marketing;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using TGS.Services.BackOffice.Application.Queries.AgentHandlers;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

public class GetMarketingRegisterDetailQuery
    : IGetService<GetMarketingRegisterDetailReportRequest, PagedResponse<MarketingRegisterDetailReportModel>>
{
    private readonly IQueryService<AllAgentsByBrandQuery, AgentInfo> _queryAgentService;
    private readonly IQueryService<GetDailyMarketingRegisterDetailQuery, PlayerDailyRegisterItem> _queryPlayerDailyRegisterService;
    private readonly ISystemClock _systemClock;
    private readonly IReportClient _reportClient;

    public GetMarketingRegisterDetailQuery(
        IReportClient reportClient,
        IQueryService<AllAgentsByBrandQuery, AgentInfo> queryAgentService,
        IQueryService<GetDailyMarketingRegisterDetailQuery, PlayerDailyRegisterItem> queryPlayerDailyRegisterService,
        ISystemClock systemClock)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _queryAgentService = queryAgentService ?? throw new ArgumentNullException(nameof(queryAgentService));
        _queryPlayerDailyRegisterService = queryPlayerDailyRegisterService ?? throw new ArgumentNullException(nameof(queryPlayerDailyRegisterService));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async ValueTask<PagedResponse<MarketingRegisterDetailReportModel>> GetAsync(
        GetMarketingRegisterDetailReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentDate = DateOnly.FromDateTime(_systemClock.UtcNow.ToOffset(TimeSpan.FromHours(8D)).DateTime);

        var requestDate = DateOnly.FromDateTime(request.Date.ToOffset(TimeSpan.FromHours(8D)).Date);

        if (requestDate == currentDate)
        {
            var agentQuery = _queryAgentService
                .QueryAsync(new AllAgentsByBrandQuery(request.BrandId), cancellationToken);

            var agentDict = await agentQuery
                .ToDictionaryAsync(
                    a => a.UserName,
                    a => a.AgentId,
                    cancellationToken)
                .ConfigureAwait(false);

            var agentId = agentDict[request.AgentUsername];

            var data = _queryPlayerDailyRegisterService
                .QueryAsync(new GetDailyMarketingRegisterDetailQuery(request.BrandId, requestDate), cancellationToken)
                .Where(item => item.AgentId == agentId)
                .Select(item => new MarketingRegisterDetailReportModel
                {
                    Username = item.UserName,
                    RegisteredOn = item.RegisteredOn,
                    LastLoginOn = item.LastLoginOn ?? DateTimeOffset.MinValue,
                    LoginCount = item.LoginCount ?? 0
                });

            var totalCount = await data.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var paging = new Paging<MarketingRegisterDetailReportModel>();

            paging.Page = request.Page;
            paging.RowCount = request.Rows;

            data = data.ApplyPaging(paging);

            return new PagedResponse<MarketingRegisterDetailReportModel>
            {
                Valid = true,
                TotalCount = totalCount,
                Items = await data
                    .ToArrayAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false)
            };
        }
        else
        {
            var reportRequest = new GetMarketingRegisterDetailRequest
            {
                BrandId = request.BrandId,
                AgentUsername = request.AgentUsername,
                Date = new DateTimeOffset(
                    requestDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified),
                    TimeSpan.FromHours(8D)),
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _reportClient.GetMarketingRegisterDetailsAsync(reportRequest);

            return new PagedResponse<MarketingRegisterDetailReportModel>
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items.Select(x => new MarketingRegisterDetailReportModel
                {
                    Username = x.Username,
                    RegisteredOn = x.RegisteredOn,
                    LastLoginOn = x.LastLoginOn,
                    LoginCount = x.LoginCount
                }).ToArray()
            };
        }
    }
}

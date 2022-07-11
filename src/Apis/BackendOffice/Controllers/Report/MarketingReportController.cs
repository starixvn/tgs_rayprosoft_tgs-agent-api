using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

namespace TGS.Apis.BackendOffice.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class MarketingReportController : BaseContextController
{
    [HttpGet("firstDepositDetail")]
    [Description("Get first deposit details report")]
    public async Task<PagedResponse<MarketingFirstDepositDetailReportModel>> GetMarketingFirstDepositDetailsAsync(
        [FromServices]
        IGetService<GetMarketingFirstDepositDetailReportRequest, PagedResponse<MarketingFirstDepositDetailReportModel>>
            marketsService,
        [FromQuery] GetMarketingFirstDepositDetailReportRequest request)
        => await marketsService
            .GetAsync(request)
            .ConfigureAwait(false);

    [HttpGet("gamingDetail")]
    [Description("Get gaming details report")]
    public async Task<PagedResponse<MarketingGamingDetailReportModel>> GetMarketingGamingDetailsAsync(
        [FromServices]
        IGetService<GetMarketingGamingDetailReportRequest, PagedResponse<MarketingGamingDetailReportModel>>
            marketsService,
        [FromQuery] GetMarketingGamingDetailReportRequest request)
        => await marketsService
            .GetAsync(request)
            .ConfigureAwait(false);

    [HttpGet]
    [Description("Get marketing report")]
    public Task<MarketingReportResponse> GetMarketingListAsync(
        [FromServices] IGetService<GetMarketingReportAggregateInfosQuery, MarketingReportResponse> queryService,
        [FromServices] IGetService<GetMarketingReportAggregateInfosByAgentQuery, MarketingReportResponse> agentQueryService,
        [FromServices] IDateLimiter dateLimiter,
        [FromQuery] GetMarketingListReportRequest request,
        [FromQuery] PageableRequest pagingRequest)
    {
        var (start, end) = dateLimiter.FutureToToday(request.From, request.To, TimeSpan.FromHours(8D));

        if (string.IsNullOrEmpty(request.AgentUsername))
        {
            var query = new GetMarketingReportAggregateInfosQuery(request.BrandId, start, end, pagingRequest);

            return queryService.GetAsync(query, HttpContext.RequestAborted).AsTask();
        }
        else
        {
            var query = new GetMarketingReportAggregateInfosByAgentQuery(
                request.BrandId,
                request.AgentUsername,
                start,
                end,
                pagingRequest);

            return agentQueryService.GetAsync(query, HttpContext.RequestAborted).AsTask();
        }
    }

    [HttpGet("registerDetail")]
    [Description("Get register details report")]
    public async Task<PagedResponse<MarketingRegisterDetailReportModel>> GetMarketingRegisterDetailsAsync(
        [FromServices]
        IGetService<GetMarketingRegisterDetailReportRequest, PagedResponse<MarketingRegisterDetailReportModel>>
            marketsService,
        [FromQuery] GetMarketingRegisterDetailReportRequest request)
        => await marketsService
            .GetAsync(request)
            .ConfigureAwait(false);
}

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

namespace TGS.Apis.BackendOffice.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class MarketingReportController : BaseContextController
{
    [HttpGet("firstDepositDetail")]
    [Description("Get first deposit details report")]
    public Task<PagedResponse<MarketingFirstDepositDetailReportModel>> GetMarketingFirstDepositDetailsAsync(
        [FromServices]
        IGetService<GetMarketingFirstDepositDetailQuery, PagedResponse<MarketingFirstDepositDetailReportModel>>
            marketsService,
        [FromQuery] GetMarketingFirstDepositDetailReportRequest request,
        [FromQuery] PageableRequest pagingRequest)
    {
        var date = DateOnly.FromDateTime(request.Date.ToOffset(TimeSpan.FromHours(8D)).DateTime);

        var query = new GetMarketingFirstDepositDetailQuery(
            request.BrandId,
            date,
            request.AgentUsername,
            pagingRequest);

        return marketsService.GetAsync(query).AsTask();
    }

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
    public async Task<MarketingReportResponse> GetMarketingListAsync(
        [FromServices] IGetService<GetMarketingReportAggregateInfosQuery, MarketingReportResponse> queryService,
        [FromServices] IGetService<GetMarketingReportAggregateInfosByAgentQuery, MarketingReportResponse> agentQueryService,
        [FromQuery] GetMarketingListReportRequest request,
        [FromQuery] PageableRequest pagingRequest)
    {
        var start = DateOnly.FromDateTime(request.From.ToOffset(TimeSpan.FromHours(8D)).DateTime);
        var end = DateOnly.FromDateTime(request.To.ToOffset(TimeSpan.FromHours(8D)).DateTime);
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8D)).DateTime);

        if (start > today)
            return new MarketingReportResponse
            {
                Items = Array.Empty<MarketingReportModel>(),
                Total = new MarketingReportTotalModel()
            };

        end = end > today ? today : end;

        if (string.IsNullOrEmpty(request.AgentUsername))
        {
            var query = new GetMarketingReportAggregateInfosQuery(request.BrandId, start, end, pagingRequest);

            return await queryService.GetAsync(query, HttpContext.RequestAborted)
                .ConfigureAwait(false);
        }
        else
        {
            var query = new GetMarketingReportAggregateInfosByAgentQuery(
                request.BrandId,
                request.AgentUsername,
                start,
                end,
                pagingRequest);

            return await agentQueryService.GetAsync(query, HttpContext.RequestAborted)
                .ConfigureAwait(false);
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

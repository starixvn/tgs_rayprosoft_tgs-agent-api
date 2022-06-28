using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using Tgs.ServiceUtility;
using TGS.Paging;
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

    //[HttpGet]
    //[Description("Get marketing report")]
    //public Task<MarketingReportResponse> GetMarketingListAsync(
    //    [FromServices] IGetService<GetMarketingListReportRequest, MarketingReportResponse>
    //        marketsService,
    //    [FromServices] IGetService<Get>
    //    [FromQuery] GetMarketingListReportRequest request)
    //{
    //    return marketsService.GetAsync(request).AsTask();
    //}

    [HttpGet]
    [Description("Get marketing report")]
    public Task<MarketingReportResponse> GetMarketingListAsync(
        [FromServices] IGetService<GetMarketingReportAggregateInfosQuery, MarketingReportResponse> queryService,
        [FromServices] IGetService<GetMarketingReportAggregateInfosByAgentQuery, MarketingReportResponse> agentQueryService,
        [FromQuery] GetMarketingListReportRequest request)
    {
        throw new NotImplementedException();
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

using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Attributes;
using TGS.Core.Models.Messages;
using Tgs.Report.Client.Models.Market;
using TGS.Services.BackOffice.Application.Models.ReportMarket;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingReportController : BaseContextController
    {
        [HttpGet]
        [Description("Get marketing report")]
        public async Task<PagedResponse<MarketingReportModel>> GetMarketingListAsync(
            [FromServices] IGetService<GetMarketingListReportRequest, PagedResponse<MarketingReportModel>>
                marketsService,
            [FromQuery] GetMarketingListReportRequest request)
            => await marketsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("registerDetail")]
        [Description("Get register details report")]
        public async Task<PagedResponse<MarketingRegisterDetailReportModel>> GetMarketingRegisterDetailsAsync(
            [FromServices] IGetService<GetMarketingRegisterDetailReportRequest, PagedResponse<MarketingRegisterDetailReportModel>>
                marketsService,
            [FromQuery] GetMarketingRegisterDetailReportRequest request)
            => await marketsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("firstDepositDetail")]
        [Description("Get first deposit details report")]
        public async Task<PagedResponse<MarketingFirstDepositDetailReportModel>> GetMarketingFirstDepositDetailsAsync(
            [FromServices] IGetService<GetMarketingFirstDepositDetailReportRequest, PagedResponse<MarketingFirstDepositDetailReportModel>>
                marketsService,
            [FromQuery] GetMarketingFirstDepositDetailReportRequest request)
            => await marketsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("gamingDetail")]
        [Description("Get gaming details report")]
        public async Task<PagedResponse<MarketingGamingDetailReportModel>> GetMarketingFirstDepositDetailsAsync(
            [FromServices] IGetService<GetMarketingGamingDetailReportRequest, PagedResponse<MarketingGamingDetailReportModel>>
                marketsService,
            [FromQuery] GetMarketingGamingDetailReportRequest request)
            => await marketsService
                .GetAsync(request)
                .ConfigureAwait(false);
    }
}
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using Tgs.Report.Client.Models.Market;
using TGS.Services.BackOffice.Application.Models.ReportMarket;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketReportController : BaseContextController
    {
        [HttpGet]
        [Description("Get market report")]
        public async Task<PagedResponse<MarketReportModel>> GetMarketsAsync(
            [FromServices] IGetService<GetMarketsReportRequest, PagedResponse<MarketReportModel>>
                marketsService,
            [FromQuery] GetMarketsReportRequest request)
            => await marketsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("additionalExpenses")]
        [Description("Get Additional Expenses")]
        public async Task<GetAdditionalExpensesReportResponse> GetAdditionalExpensesAsync(
            [FromServices] IGetService<GetAdditionalExpensesReportRequest, GetAdditionalExpensesReportResponse>
                additionalExpensesService,
            [FromQuery] GetAdditionalExpensesReportRequest request)
            => await additionalExpensesService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("exportMarkets")]
        public async Task<IActionResult> ExportMarketsAsync(
            [FromServices] IGetService<ExportMarketsRequest, Stream>
                exportMarketsService,
            [FromQuery] ExportMarketsRequest request) => File(
            await exportMarketsService
                .GetAsync(request)
                .ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Markets.xlsx");
    }
}
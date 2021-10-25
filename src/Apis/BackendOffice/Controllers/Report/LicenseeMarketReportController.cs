using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.LicenseeMarket;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseeMarketReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public LicenseeMarketReportController(IReportClient client)
        {
            _client = client;
        }

        [HttpGet("exportLicenseeMarkets")]
        [Description(" Export licensee markets")]
        public async Task<IActionResult> ExportMarketsAsync([FromQuery] ExportLicenseeMarketsRequest request) => File(
            await _client.ExportLicenseeMarketsAsync(request).ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LicenseeMarkets.xlsx");

        [HttpGet]
        [Description("Get licensee markets")]
        public Task<LicenseeMarketsResponse> GetLicenseeMarketsAsync([FromQuery] GetLicenseeMarketsRequest request) =>
            _client.GetLicenseeMarketsAsync(request);
    }
}

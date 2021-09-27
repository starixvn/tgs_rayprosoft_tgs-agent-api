using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Market;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public MarketReportController(IReportClient client)
        {
            _client = client;
        }

        [HttpGet]
        [Description("Get market report")]
        public Task<PagedResponse<MarketModel>> GetMarketsAsync([FromQuery] GetMarketsRequest request) =>
            _client.GetMarketsAsync(request);

        [HttpGet("exportMarkets")]
        public async Task<IActionResult> ExportMarketsAsync([FromQuery] ExportMarketsRequest request) => File(
            await _client.ExportMarketsAsync(request).ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Markets.xlsx");
    }
}
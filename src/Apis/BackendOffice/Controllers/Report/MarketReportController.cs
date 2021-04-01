using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Report.Client;
using Report.Client.Models.Market;
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
    }
}
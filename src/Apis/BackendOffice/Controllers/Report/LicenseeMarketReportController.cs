using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.LicenseeMarket;
using TGS.Core.Models.Messages;

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

        [HttpGet]
        [Description("Get licensee market report")]
        public Task<PagedResponse<LicenseeMarketModel>> GetLicenseeMarketsAsync([FromQuery] GetLicenseeMarketsRequest request) =>
            _client.GetLicenseeMarketsAsync(request);
    }
}

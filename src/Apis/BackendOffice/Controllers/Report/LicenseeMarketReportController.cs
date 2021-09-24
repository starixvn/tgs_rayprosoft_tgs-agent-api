using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.LicenseeMarket;

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

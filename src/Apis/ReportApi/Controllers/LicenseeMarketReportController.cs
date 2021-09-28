using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Services.Report.Application.Models.LicenseeMarket;

namespace TGS.Apis.ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseeMarketReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LicenseeMarketReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get licensee markets")]
        public Task<LicenseeMarketsResponse> GetLicenseeMarketAsync([FromQuery] GetLicenseeMarketsRequest request) =>
            _mediator.Send(request);
    }
}

using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Market;

namespace TGS.Apis.ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MarketReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get markets")]
        public Task<PagedResponse<MarketModel>> GetVipsAsync([FromQuery] GetMarketsRequest request) =>
            _mediator.Send(request);
    }
}
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
        public Task<PagedResponse<MarketModel>> GetMarketsAsync([FromQuery] GetMarketsRequest request) =>
            _mediator.Send(request);

        //[HttpGet("export")]
        //[Description("export markets")]
        //public async Task<PagedResponse<MarketModel>> ExportMarketsAsync([FromQuery] GetMarketsRequest request)
        //{
        //    var getMarketsRequest = new GetMarketsRequest
        //    {
        //        BrandId = request.BrandId,
        //        From = request.From,
        //        To = request.To,
        //        ShowAll = true
        //    };
        //    var markets = await _mediator.Send(getMarketsRequest).ConfigureAwait(false);

        //    markets.Items.(x => x.)
        //}
    }
}
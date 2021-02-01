using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Market;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : BaseContextController
    {
        private readonly IMediator _mediator;

        public MarketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get market report")]
        public Task<PagedResponse<MarketModel>> GetMarketsAsync([FromQuery] GetMarketsRequest request) =>
            _mediator.Send(request);
    }
}
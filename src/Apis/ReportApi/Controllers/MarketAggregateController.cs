using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Commands.MarketAggregate;

namespace TGS.Apis.ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketAggregateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MarketAggregateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Description("Create market aggregate")]
        public Task<MessageResponse> GetVipsAsync([FromQuery] CreateMarketAggregateModel request) =>
            _mediator.Send(request);
    }
}
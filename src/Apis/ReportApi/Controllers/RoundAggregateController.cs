using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Report.Client.Models.Aggregate;
using TGS.Core.Models.Messages;

namespace ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundAggregateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoundAggregateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Description("Create round aggregate")]
        public Task<MessageResponse> CreateRoundAggregateAsync() =>
            _mediator.Send(new CreateUpdateRoundAggregateModel());

        [HttpPost("manual")]
        [Description("Manual create or update round aggregate")]
        public Task<MessageResponse> ManualCreateUpdateRoundAggregateAsync(
            ManualCreateUpdateRoundAggregateModel request) =>
            _mediator.Send(request);
    }
}
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class PluggableController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PluggableController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("gameurl")]
        public Task<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request) =>
            _mediator.Send(request);

        [HttpPost]
        [Route("history/round")]
        public Task<GetRoundHistoryResponse> GetRoundHistory(GetRoundHistoryRequest request) =>
            _mediator.Send(request);

        [HttpPost]
        [Route("player/kick")]
        public Task<PlayerKickResponse> KickPlayerAsync(PlayerKickRequest request) =>
            _mediator.Send(request);
    }
}
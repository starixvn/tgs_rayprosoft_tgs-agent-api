using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public Task<GetGamesResponse> GetGamesAsync(GetGamesRequest request) =>
            _mediator.Send(request);

        [HttpPost]
        [Route("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request) =>
            _mediator.Send(request);
    }
}
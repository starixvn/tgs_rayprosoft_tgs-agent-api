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
    [Produces(MediaTypeNames.Application.Json)]
    public class GameProviderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameProviderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Get game provider list
        /// </summary>
        [HttpGet]
        [Route("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public Task<GetGameProvidersResponse> GetGameProvidersAsync([FromQuery] GetGameProvidersRequest request) =>
            _mediator.Send(request);
    }
}
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.GlobalApi.Models.Data;
using TGS.Apis.GlobalApi.Models.Messages;

namespace TGS.Apis.GlobalApi.Controllers
{
    [Route("global-api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class BrandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("apikeys")]
        public async ValueTask<PagedResponse<BrandApiKeyResult>> GetBrandApiKeysAsync(
            [FromQuery] GetBrandApiKeysRequest request)
            => await _mediator.Send(request).ConfigureAwait(false);

        [HttpGet]
        [AllowAnonymous]
        [Route("players/mocksite")]
        public async ValueTask<PagedResponse<BrandPlayersResult>> GetBrandPlayersForMockSiteAsync(
            [FromQuery] GetBrandPlayersRequest request)
            => await _mediator.Send(request).ConfigureAwait(false);

        /// <summary>
        ///     This api will create a new player when the player is not exist, and login with desktop
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("player/createandlogin")]
        public async ValueTask<CreatePlayerAndLoginResponse> CreatePlayerAndLoginAsync(
            CreatePlayerAndLoginRequest request)
            => await _mediator.Send(request).ConfigureAwait(false);
    }
}
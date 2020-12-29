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
        public Task<PagedResponse<BrandApiKeyResult>> GetBrandApiKeysAsync([FromQuery] GetBrandApiKeysRequest request)
            => _mediator.Send(request);

        [HttpGet]
        [AllowAnonymous]
        [Route("players/mocksite")]
        public Task<PagedResponse<BrandPlayersResult>> GetBrandPlayersForMockSiteAsync([FromQuery] GetBrandPlayersRequest request)
            => _mediator.Send(request);
    }
}

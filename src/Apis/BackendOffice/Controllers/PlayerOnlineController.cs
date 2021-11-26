using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.PlayerOnline;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerOnlineController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlayerOnlineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get Player Online ")]
        public ValueTask<PagedResponse<PlayerOnlineModel>> GetPlayerIpsListAsync(
            [FromServices] IGetService<GetPlayerOnlineRequest, PagedResponse<PlayerOnlineModel>> getService,
            [FromQuery] GetPlayerOnlineRequest request)
            => getService.GetAsync(request);
    }
}

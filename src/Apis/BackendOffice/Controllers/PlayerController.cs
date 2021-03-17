using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PlayerController : BaseContextController
    {
        private readonly IMediator _mediator;

        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get player list")]
        public Task<PagedResponse<PlayersModel>> GetListAsync([FromQuery] GetPlayersRequest request) =>
            _mediator.Send(request);

        [HttpGet("{playerCode}")]
        [Description("Get player")]
        public Task<GetPlayerResponse> GetSingleAsync(string playerCode) =>
            _mediator.Send(new GetPlayerRequest(playerCode));

        [HttpGet("{playerCode}/operations")]
        [Description("Get player operation")]
        public Task<PagedResponse<PlayerOperationModel>> GetPlayerOperationsAsync(string playerCode,
            [FromQuery] GetPlayerOperationRequest request)
        {
            request.PlayerCode = playerCode;
            return _mediator.Send(request);
        }

        [HttpPut("{playerCode}/{type}")]
        [Description("Update player")]
        public Task<MessageResponse> UpdateAsync(string playerCode, string type,
            [FromBody] UpdatePlayerRequest request)
        {
            request.PlayerCode = playerCode;
            request.Type = type;
            return _mediator.Send(request);
        }
    }
}
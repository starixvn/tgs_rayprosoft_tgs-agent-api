using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Game;
using TGS.Services.BackOffice.Application.Models.GameProvider;

namespace TGS.Apis.BackendOffice.Controllers.GameProvider
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : BaseContextController
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get games")]
        public Task<PagedResponse<GameModel>>
            GetGameProvidersAsync([FromQuery] GetGamesRequest request) => _mediator.Send(request);

        [HttpPost]
        [Description("Create games")]
        public Task<CreateGamesResponse> CreateGamesAsync(CreateGamesRequest request) =>
            _mediator.Send(request);

        [HttpPut("{id}")]
        [Description("Update a game")]
        public Task<MessageResponse> UpdateGameAsync(int id, UpdateGameRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpPost("upload")]
        [Description("Upload games")]
        public Task<UploadGamesResponse> UploadGamesAsync([FromForm] UploadGamesRequest request) =>
            _mediator.Send(request);
    }
}
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.GameProvider;

namespace TGS.Apis.BackendOffice.Controllers.GameProvider
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class GameProviderController : BaseContextController
    {
        private readonly IMediator _mediator;

        public GameProviderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get game providers")]
        public Task<PagedResponse<GameProviderModel>>
            GetGameProvidersAsync([FromQuery] GetGameProvidersRequest request) => _mediator.Send(request);

        [HttpGet("{id}")]
        [Description("Get a game provider")]
        public Task<GetGameProviderResponse> GetGameProviderAsync(Guid id) =>
            _mediator.Send(new GetGameProviderRequest(id));

        [HttpPost]
        [Description("Create a game provider")]
        public Task<MessageResponse> CreateBrandAsync(CreateGameProviderRequest request) => _mediator.Send(request);

        [HttpPut("{id}")]
        [Description("Update a game provider")]
        public Task<MessageResponse> UpdateBrandAsync(Guid id, UpdateGameProviderRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }
    }
}
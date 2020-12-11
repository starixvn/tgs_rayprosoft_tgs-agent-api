using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGame;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandGameController : BaseContextController
    {
        private readonly IMediator _mediator;

        public BrandGameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get brands: 平台商遊戲列表")]
        public Task<PagedResponse<BrandGameModel>> GetBrandGamesAsync([FromQuery] GetBrandGamesRequest request)
        {
            return _mediator.Send(request);
        }

        [HttpGet("{id}")]
        [Description("Get a brand game")]
        public Task<GetBrandGameResponse> GetBrandAsync(Guid id)
        {
            return _mediator.Send(new GetBrandGameRequest(id));
        }

        [HttpPut("{id}")]
        [Description("Update a brand game")]
        public Task<MessageResponse> UpdateBrandGameAsync(Guid id, UpdateBrandGameRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpPut("{id}/{tag}/{enable:bool}")]
        [Description("Change a brand game tag")]
        public Task<MessageResponse> UpdateBrandTagAsync(Guid id, GameTag tag, bool enable)
        {
            return _mediator.Send(new UpdateBrandGameTagRequest(id, tag, enable));
        }
    }
}
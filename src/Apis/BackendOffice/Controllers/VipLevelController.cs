using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.VipLevel;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipLevelController : BaseContextController
    {
        private readonly IMediator _mediator;

        public VipLevelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get vip levels")]
        public Task<PagedResponse<VipLevelModel>> GetListAsync([FromQuery] GetVipLevelsRequest request) =>
            _mediator.Send(request);

        [HttpPut("{id}")]
        [Description("Update a vip level")]
        public Task<MessageResponse> UpdateAsync(Guid id, [FromBody] UpdateVipLevelRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpPost]
        [Description("Create a vip level")]
        public Task<MessageResponse> CreateAsync([FromBody] CreateVipLevelRequest request) => _mediator.Send(request);
    }
}
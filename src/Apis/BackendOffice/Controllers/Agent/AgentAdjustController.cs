using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Application.Models.AgentAdjust;

namespace TGS.Apis.BackendOffice.Controllers.Agent
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentAdjustController : BaseContextController
    {
        private readonly IMediator _mediator;

        public AgentAdjustController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get agents")]
        public Task<PagedResponse<AgentModel>> GetAgentAdjustsAsync([FromQuery] GetAgentsRequest request) =>
            _mediator.Send(request);

        [HttpGet("{id}")]
        [Description("Get an agent")]
        public Task<GetAgentResponse> GetAgentAdjustAsync(Guid id) => _mediator.Send(new GetAgentRequest(id));

        [HttpPost("{id}/adjust")]
        [Description("Create an agent adjustment")]
        public Task<MessageResponse> CreateAgentAdjustmentAsync(Guid id, CreateAgentAdjustmentRequest request)
        {
            request.AgentId = id;
            return _mediator.Send(request);
        }
            
    }
}
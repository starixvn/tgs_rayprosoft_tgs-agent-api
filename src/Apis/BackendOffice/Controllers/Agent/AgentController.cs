using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.Report.Application.Models.Player;

namespace TGS.Apis.BackendOffice.Controllers.Agent
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class AgentController : BaseContextController
    {
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get agents")]
        public Task<PagedResponse<AgentModel>> GetAgentsAsync([FromQuery] GetAgentsRequest request) =>
            _mediator.Send(request);

        [HttpGet("{id}")]
        [Description("Get an agent")]
        public Task<GetAgentResponse> GetAgentAsync(Guid id) => _mediator.Send(new GetAgentRequest(id));

        [HttpGet("{id}/operations")]
        [Description("Get agent operations")]
        public Task<PagedResponse<AgentOperationModel>> GetAgentOperationsAsync(Guid id) =>
            _mediator.Send(new GetAgentOperationsRequest(id));

        [HttpPost]
        [Description("Create a agent")]
        public Task<MessageResponse> CreateAgentAsync(CreateAgentRequest request) => _mediator.Send(request);

        [HttpPut("{id}")]
        [Description("Update a agent")]
        public Task<MessageResponse> UpdateAgentAsync(Guid id, UpdateAgentRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpGet("{id}/teamInfo")]
        [Description("Get agent team Information")]
        public Task<PagedResponse<PlayerModel>> GetAgentTeamInformationAsync(Guid id) =>
            _mediator.Send(new GetAgentTeamInfoRequest(id));
    }
}
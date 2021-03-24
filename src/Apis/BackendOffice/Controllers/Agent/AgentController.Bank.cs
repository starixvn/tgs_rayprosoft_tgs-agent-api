using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;

namespace TGS.Apis.BackendOffice.Controllers.Agent
{
    public partial class AgentController
    {
        [HttpPost("{agentId}/bank")]
        [Description("Create a Agent bank")]
        public Task<MessageResponse> CreateAgentBankAsync(Guid agentId, [FromQuery] CreateAgentBankRequest request)
        {
            request.AgentId = agentId;
            return _mediator.Send(request);
        }

        [HttpPut("{agentId}/bank/{id}")]
        [Description("Update a Agent bank")]
        public Task<MessageResponse> UpdateAgentBankAsync(Guid agentId, Guid id,
            [FromQuery] UpdateAgentBankRequest request)
        {
            request.AgentId = agentId;
            request.Id = id;
            return _mediator.Send(request);
        }
    }
}
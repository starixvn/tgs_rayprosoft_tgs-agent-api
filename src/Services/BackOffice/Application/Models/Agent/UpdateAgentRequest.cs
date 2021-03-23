using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Agent
{
    public class UpdateAgentRequest : AgentRequest, IRequest<MessageResponse>
    {
        public Guid Id { get; set; }
    }
}
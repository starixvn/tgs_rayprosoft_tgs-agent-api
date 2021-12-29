using System;

namespace TGS.Services.Agent.ApplicationServices.Agent
{
    public record ValidateAgentPasswordCommand(Guid AgentId, string Password);
}

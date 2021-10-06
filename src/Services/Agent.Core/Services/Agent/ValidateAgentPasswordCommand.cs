using System;

namespace TGS.Services.Agent.Services.Agent
{
    public record ValidateAgentPasswordCommand(Guid AgentId, string Password);
}

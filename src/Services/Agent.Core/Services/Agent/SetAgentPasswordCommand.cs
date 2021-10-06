using System;

namespace TGS.Services.Agent.Services.Agent
{
    public record SetAgentPasswordCommand(Guid AgentId, string NewPassword);
}

using System;

namespace TGS.Services.Agent.Services.Agent
{
    public record CommitSetAgentEmailCommand(Guid AgentId, string SecurityKey, string ValidateCode);
}

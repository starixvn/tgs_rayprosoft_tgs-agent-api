using System;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record CommitSetAgentEmailCommand(Guid AgentId, string SecurityKey, string ValidateCode);

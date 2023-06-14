using System;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

public record ResetSubAgentPasswordCommand(Guid AgentId, Guid SubAgentId, string Password);

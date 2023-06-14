using System;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

public record SetSubAgentContactCommand(
    Guid AgentId,
    Guid SubAgentId,
    string? Email,
    string? Phone);

using System;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

public record SetSubAgentInfoCommand(
    Guid AgentId,
    Guid SubAgentId,
    string? RealName);

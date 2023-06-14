using System;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

public record SetSubAgentDividendPercentageCommand(
    Guid BrandId,
    Guid AgentId,
    Guid SubAgentId,
    decimal DividendPercentage);

using System;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

public record GetSubAgentDividendRangeQuery(Guid AgentId, Guid SubAgentId);

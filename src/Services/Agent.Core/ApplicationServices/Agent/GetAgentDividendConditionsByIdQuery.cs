using System;

namespace TGS.Services.Agent.ApplicationServices.Agent
{
    public record GetAgentDividendConditionsByIdQuery(Guid BrandId, Guid AgentId);
}

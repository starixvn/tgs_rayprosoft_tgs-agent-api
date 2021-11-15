using System;

namespace TGS.Services.BackOffice.Application.Services
{
    public record AddAgentDomainCommand(Guid BrandId, Guid AgentId, string DomainName);
}

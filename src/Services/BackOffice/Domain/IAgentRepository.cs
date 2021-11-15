using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public interface IAgentRepository
    {
        ValueTask AddDomainToAgentAsync(Guid agentId, string domainName);

        IAsyncEnumerable<string> QueryDomainsFromAgentAsync(Guid agentId);

        ValueTask RemoveDomainFromAgentAsync(Guid agentId, string domainName);
    }
}

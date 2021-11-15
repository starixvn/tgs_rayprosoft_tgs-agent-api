using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Services;

namespace TGS.Apis.BackendOffice.Controllers.Agent
{
    [Route("api/agent/domain")]
    [ApiController]
    public class AgentDomainController : BaseContextController
    {
        [HttpPut("{brandId}/{agentId}")]
        public ValueTask AddAgentDomainAsync(
            [FromServices] ICommandService<AddAgentDomainCommand> addService,
            Guid brandId,
            Guid agentId,
            [RegularExpression(".{1,253}"), Required, FromQuery(Name = "domain")] string domainName)
        {
            var command = new AddAgentDomainCommand(agentId, domainName);

            return addService.ExecuteAsync(command);
        }

        [HttpGet("{brandId}/{agentId}")]
        public IAsyncEnumerable<string> GetAgentDomainsAsync(
            [FromServices] IQueryService<AgentDomainsQuery, string> queryService,
            Guid brandId,
            Guid agentId)
        {
            var query = new AgentDomainsQuery(agentId);

            return queryService.QueryAsync(query);
        }

        [HttpDelete("{brandId}/{agentId}")]
        public ValueTask RemoveAgentDomainAsync(
            [FromServices] ICommandService<RemoveAgentDomainCommand> removeService,
            Guid brandId,
            Guid agentId,
            [RegularExpression(".{1,253}"), Required, FromQuery(Name = "domain")] string domainName)
        {
            var command = new RemoveAgentDomainCommand(agentId, domainName);

            return removeService.ExecuteAsync(command);
        }
    }
}

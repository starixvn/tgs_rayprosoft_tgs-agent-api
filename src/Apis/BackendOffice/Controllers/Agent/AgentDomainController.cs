using System;
using System.ComponentModel.DataAnnotations;
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
        public System.Threading.Tasks.ValueTask AddAgentDomainAsync(
            [FromServices] ICommandService<AddAgentDomainCommand> addService,
            Guid brandId,
            Guid agentId,
            [RegularExpression(".{1,253}"), Required, FromQuery(Name = "domain")] string domainName)
        {
            var command = new AddAgentDomainCommand(brandId, agentId, domainName);

            return addService.ExecuteAsync(command);
        }
    }
}

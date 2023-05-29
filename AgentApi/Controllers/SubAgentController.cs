using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Services.Agent.ApplicationServices.SubAgent;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class SubAgentController : ControllerBase
{
    [HttpPost("resetPwd", Name = "ResetSubAgentPassword")]
    public async ValueTask ResetSubAgentPasswordAsync(
        [FromServices] ICommandService<ResetSubAgentPasswordCommand> commandService,
        ResetSubAgentPasswordRequest request)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentId = agentTokenData.AgentId;

        await commandService
            .ExecuteAsync(new ResetSubAgentPasswordCommand(agentId, request.SubAgentId, request.Password), HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }

    [HttpPost("setInfo", Name = "SetSubAgentInfoAsync")]
    public async ValueTask SetSubAgentInfoAsync(
        [FromServices] ICommandService<SetSubAgentInfoCommand> commandService,
        SetSubAgentInfoRequest request)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentId = agentTokenData.AgentId;

        await commandService
            .ExecuteAsync(new SetSubAgentInfoCommand(agentId, request.SubAgentId, request.RealName, request.Email, request.Phone), HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }
}

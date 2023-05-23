using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.AgentTeam;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Operations.Validators;
using Tgs.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentTeamController : ControllerBase
{
    [HttpGet("info")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<AgentTeamResponse> GetAgentTeamInfoAsync(
        [FromServices] IStatusValidator statusValidator,
        [FromServices] IGetService<GetAgentTeamInfoQuery, GetAgentTeamInfoModel> getAgentService)
    {
        var brandId = HttpContext.Items.GetBrandId();
        var agentData = HttpContext.Items.GetAgentTokenData();

        await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

        var agentTeamInfo = await getAgentService
            .GetAsync(new(brandId, agentData.AgentId))
            .ConfigureAwait(false) ?? throw new BrandNotFoundException();

        return new()
        {
            Data = new()
            {
                DirectAgentCount = agentTeamInfo.DirectAgentCount,
                DirectPlayerCount = agentTeamInfo.DirectPlayerCount,
                IndirectAgentCount = agentTeamInfo.IndirectAgentCount,
                IndirectPlayerCount = agentTeamInfo.IndirectPlayerCount
            }
        };
    }
}
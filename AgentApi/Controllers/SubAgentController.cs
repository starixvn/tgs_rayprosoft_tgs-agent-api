using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Services.Agent.ApplicationServices.SubAgent;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class SubAgentController : ControllerBase
{
    [HttpGet("{subAgentId}/dividendrange", Name = "GetSubAgentDividendRange")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async ValueTask<(decimal, decimal)> GetSubAgentDividendRangeAsync(
        [FromServices] IGetService<GetSubAgentDividendRangeQuery, (decimal, decimal)> getService,
        Guid subAgentId)
    {
        var agentToken = HttpContext.Items.GetAgentTokenData();

        return await getService
            .GetAsync(new GetSubAgentDividendRangeQuery(agentToken.AgentId, subAgentId))
            .ConfigureAwait(false);
    }

    [HttpPatch("password", Name = "ResetSubAgentPassword")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public ValueTask ResetSubAgentPasswordAsync(
        [FromServices] ICommandService<ResetSubAgentPasswordCommand> commandService,
        [FromBody] ResetSubAgentPasswordRequest request)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentId = agentTokenData.AgentId;

        return commandService
             .ExecuteAsync(new ResetSubAgentPasswordCommand(agentId, request.SubAgentId, request.Password), HttpContext.RequestAborted);
    }

    [HttpPatch("dividend", Name = "SetSubAgentDividendPercentage")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public ValueTask SetSubAgentDividendPercentageAsync(
        [FromServices] ICommandService<SetSubAgentDividendPercentageCommand> commandService,
        [FromBody] SetSubAgentDividendPercentageRequest request)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentId = agentTokenData.AgentId;
        var brandId = HttpContext.Items.GetBrandId();

        return commandService
            .ExecuteAsync(new SetSubAgentDividendPercentageCommand(brandId, agentId, request.SubAgentId, request.DividendPercentage));
    }

    [HttpPatch("info", Name = "SetSubAgentInfo")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async ValueTask SetSubAgentInfoAsync(
        [FromServices] ICommandService<SetSubAgentInfoCommand> commandService,
        [FromBody] SetSubAgentInfoRequest request)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentId = agentTokenData.AgentId;

        await commandService
            .ExecuteAsync(new SetSubAgentInfoCommand(agentId, request.SubAgentId, request.RealName, request.Email, request.Phone), HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }
}

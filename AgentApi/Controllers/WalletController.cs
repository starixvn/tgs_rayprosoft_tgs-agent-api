using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Client;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Models.Exceptions;
using TGS.Services.Agent.ApplicationServices.Agent;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    /// <summary>
    /// Gets the balance information.
    /// </summary>
    /// <param name="getBalanceService">The get balance service.</param>
    /// <returns></returns>
    [HttpGet]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    [TgsResponseWrapper]
    public async Task<BalanceInfoResponse> GetBalanceAsync(
        [FromServices] IAgentClient agentClient,
        [FromServices] IGetService<GetAgentBalanceQuery, decimal> getAgentBalanceService)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentInfo = await agentClient
            .GetAgentInfoAsync(new Tgs.Agent.Client.Models.GetAgentInfoRequest(agentTokenData.AgentId))
            .ConfigureAwait(false)
            ?? throw new AgentNotFoundException(agentTokenData.AgentId.ToString(), Resources.Agent.NotFound);

        if (agentInfo.WalletId is null)
            throw new Exception("Wallet Not Found.");

        var balance = await getAgentBalanceService
            .GetAsync(new GetAgentBalanceQuery(agentInfo.WalletId.Value, agentInfo.Id))
            .ConfigureAwait(false);

        return new BalanceInfoResponse
        {
            Main = balance
        };
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Agent;
using TGS.Services.Agent.ApplicationServices.Wallet;

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
        [FromServices] IGetService<GetAgentInfoQuery, AgentInfo> getAgentService,
        [FromServices] IGetService<GetAgentBalanceQuery, decimal> getAgentBalanceService)
    {
        var agentTokenData = HttpContext.Items.GetAgentTokenData();

        var agentInfo = await getAgentService
                            .GetAsync(new GetAgentInfoQuery(agentTokenData.AgentId))
                            .ConfigureAwait(false)
                        ?? throw new Core.Models.Exceptions.AgentNotFoundException(agentTokenData.AgentId.ToString(), Resources.Agent.NotFound);

        var balance = await getAgentBalanceService
            .GetAsync(new GetAgentBalanceQuery(agentInfo.DefaultWalletId, agentInfo.Id))
            .ConfigureAwait(false);

        return new BalanceInfoResponse
        {
            Main = balance
        };
    }
}

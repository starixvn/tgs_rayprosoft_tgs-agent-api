using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Wallet;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
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
            [FromServices] IGetService<GetBalanceQuery, BalanceInfo> getBalanceService)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var query = new GetBalanceQuery(agentTokenData.AgentId);

            var info = await getBalanceService
                .GetAsync(query)
                .ConfigureAwait(false);

            return new BalanceInfoResponse()
            {
                Main = info.Main
            };
        }
    }
}

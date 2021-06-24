using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent.Services.Agent;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentEmailController : ControllerBase
    {
        /// <summary>
        /// Starts the change email.
        /// </summary>
        /// <param name="setEmailService">The set email service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask<string> StartChangeEmailAsync(
            [FromServices] IGetService<StartSetAgentEmailCommand, string> setEmailService,
            [FromBody] StartSetEmailRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var command = new StartSetAgentEmailCommand(
                agentTokenData.AgentId,
                request.Email,
                CultureInfo.CurrentUICulture);

            return setEmailService.GetAsync(command);
        }
    }
}

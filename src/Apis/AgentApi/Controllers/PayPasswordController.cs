using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent.Services.PayPassowrd;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/pay/password")]
    [ApiController]
    public class PayPasswordController : ControllerBase
    {
        [HttpPost]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task SetPayPasswordAsync(
            [FromServices] ICommandService<SetFirstPayPasswordCommand> setPayPasswordService,
            [FromBody] SetPayPasswordRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var command = new SetFirstPayPasswordCommand(agentTokenData.AgentId, request.Password);

            await setPayPasswordService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpPost("validate-code")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask<string> StartChangePayPasswordAsync(
            [FromServices] IGetService<StartChangePayPasswordCommand, string> startChangePayPasswordService)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var command = new StartChangePayPasswordCommand(agentTokenData.AgentId, CultureInfo.CurrentUICulture);

            return startChangePayPasswordService.GetAsync(command);
        }

        [HttpPatch]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask ChangePayPasswordAsync(
            [FromServices] ICommandService<ChangePayPasswordCommand> changePayPasswordService,
            ChangePayPasswordRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var command = new ChangePayPasswordCommand(
                agentTokenData.AgentId,
                request.NewPassword,
                request.ValidateCode,
                request.SecurityKey);

            return changePayPasswordService.ExecuteAsync(command);
        }
    }
}

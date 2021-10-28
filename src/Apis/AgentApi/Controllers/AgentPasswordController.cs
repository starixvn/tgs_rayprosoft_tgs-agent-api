using System.Globalization;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent.Services.Agent;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/agent/password")]
    [ApiController]
    public class AgentPasswordController : ControllerBase
    {
        /// <summary>
        /// Agent changes password, agent login is needed when use this
        /// </summary>
        [HttpPut]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task ChangePasswordAsync(
            [FromServices] IGetService<ValidateAgentPasswordCommand, bool> validateService,
            [FromServices] ICommandService<SetAgentPasswordCommand> setPasswordService,
            [FromBody] ChangePasswordRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var validateCommand = new ValidateAgentPasswordCommand(agentTokenData.AgentId, request.OldPassword);

            var isValid = await validateService
                .GetAsync(validateCommand)
                .ConfigureAwait(false);

            if (!isValid)
                throw new PasswordInvalidException();

            var command = new SetAgentPasswordCommand(
                agentTokenData.AgentId,
                request.NewPassword);

            await setPasswordService
                .ExecuteAsync(command)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Start forgot password flow.
        /// </summary>
        /// <param name="forgotPasswordService"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot")]
        [AuthorizeAgentToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ValueTask<string> ForgotPasswordAsync(
            [FromServices] IGetService<ForgotPasswordCommand, string> forgotPasswordService,
            [FromBody] ForgotPasswordRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();

            return forgotPasswordService
                .GetAsync(new ForgotPasswordCommand(
                    brandId,
                    CultureInfo.CurrentUICulture,
                    request.Email));
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="resetService"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset")]
        [AuthorizeAgentToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask ResetPasswordAsync(
            [FromServices] ICommandService<ResetPasswordCommand> resetService,
            [FromBody] ResetPasswordRequest request)
        {
            await resetService
                .ExecuteAsync(new ResetPasswordCommand(request.SecurityKey, request.ValidateCode, request.NewPassword))
                .ConfigureAwait(false);
        }
    }
}

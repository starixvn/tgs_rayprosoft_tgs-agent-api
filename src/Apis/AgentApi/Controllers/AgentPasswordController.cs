using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent.Agent;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/agent/password")]
    [ApiController]
    public class AgentPasswordController : ControllerBase
    {
        /// <summary>
        /// Start forgot password flow.
        /// </summary>
        /// <param name="forgotPasswordService"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot")]
        [AuthorizeAgentToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [TgsResponseWrapper]
        public ValueTask<string> ForgotPasswordAsync(
            [FromServices] IGetService<ForgotPasswordCommand, string> forgotPasswordService,
            [FromBody] ForgotPasswordRequest request)
        {
            return forgotPasswordService
                .GetAsync(new ForgotPasswordCommand(CultureInfo.CurrentUICulture, request.Email));
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
        [TgsResponseWrapper]
        public async ValueTask ResetPasswordAsync([FromServices] ICommandService<ResetPasswordCommand> resetService,
            [FromBody] ResetPasswordRequest request)
        {
            await resetService
                .ExecuteAsync(new ResetPasswordCommand(request.SecurityKey, request.ValidateCode, request.NewPassword))
                .ConfigureAwait(false);
        }
    }
}
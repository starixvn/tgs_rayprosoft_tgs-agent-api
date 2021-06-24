using System.Globalization;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Player;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/player/password")]
    [ApiController]
    public class PlayerPasswordController : ControllerBase
    {
        /// <summary>
        /// Player changes password, player login is needed when use this
        /// </summary>
        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ChangePasswordResponse> ChangePasswordAsync(
            [FromServices] IGetService<GetChangePasswordQuery, ChangePasswordInfo> getChangePasswordQueryService,
            [FromBody] ChangePasswordRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetChangePasswordQuery(
                brandId,
                playerTokenData.PlayerCode,
                request.OldPassword,
                request.NewPassword);

            ChangePasswordInfo info;
            try
            {
                info = await getChangePasswordQueryService
                    .GetAsync(query)
                    .ConfigureAwait(false);
            }
            catch (Core.Models.Exceptions.InvalidCredentialsException)
            {
                throw new Core.Models.Exceptions.InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            return new ChangePasswordResponse()
            {
                PlayerCode = info.PlayerCode,
                UpdateTime = info.UpdateTime
            };
        }

        /// <summary>
        /// Start forgot password flow.
        /// </summary>
        /// <param name="forgotPasswordService">The forgot password service.</param>
        /// <param name="request">The request.</param>
        /// <response code="200">Return the token, when reset password, must input to verify.</response>
        [HttpPost("forgot")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
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
        /// <param name="resetService">The reset service.</param>
        /// <param name="request">The request.</param>
        /// <response code="406">Reset password not accept, maybe validate code not match.</response>
        [HttpPost("reset")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [TgsResponseWrapper]
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
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.Player;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/player/email")]
    [ApiController]
    public class PlayerEmailController : ControllerBase
    {
        /// <summary>
        /// Starts the change email.
        /// </summary>
        /// <param name="setPlayerEmailService">The set player email service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask<string> StartChangeEmailAsync(
            [FromServices] IGetService<StartSetPlayerEmailCommand, string> setPlayerEmailService,
            [FromBody] StartSetEmailRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new StartSetPlayerEmailCommand(playerTokenData.PlayerCode, request.Email, CultureInfo.CurrentUICulture);

            return setPlayerEmailService.GetAsync(command);
        }

        /// <summary>
        /// Submits the change email.
        /// </summary>
        /// <param name="commitPlayerEmailService">The commit player email service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask SubmitChangeEmailAsync(
            [FromServices] ICommandService<CommitSetPlayerEmailCommand> commitPlayerEmailService,
            [FromBody] SubmitChangeEmailRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new CommitSetPlayerEmailCommand(playerTokenData.PlayerCode, request.SecurityKey, request.ValidateCode);

            return commitPlayerEmailService.ExecuteAsync(command);
        }
    }
}
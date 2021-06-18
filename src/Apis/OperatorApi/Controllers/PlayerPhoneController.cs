using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.Services.Player;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/player/phone")]
    [ApiController]
    public class PlayerPhoneController : ControllerBase
    {
        /// <summary>
        ///     Starts the change phone.
        /// </summary>
        /// <param name="startSetPlayerPhoneService">The start set player phone service.</param>
        /// <param name="request">The request.</param>
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask<string> StartChangePhoneAsync(
            [FromServices] IGetService<StartSetPlayerPhoneCommand, string> startSetPlayerPhoneService,
            [FromBody] StartSetPhoneRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new StartSetPlayerPhoneCommand(playerTokenData.PlayerCode, request.Phone,
                CultureInfo.CurrentUICulture);

            return startSetPlayerPhoneService.GetAsync(command);
        }

        /// <summary>
        ///     Submits the change phone.
        /// </summary>
        /// <param name="commitPlayerPhoneService">The commit player phone service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask SubmitChangePhoneAsync(
            [FromServices] ICommandService<CommitSetPlayerPhoneCommand> commitPlayerPhoneService,
            [FromBody] SubmitChangePhoneRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new CommitSetPlayerPhoneCommand(
                playerTokenData.PlayerCode,
                request.SecurityKey,
                request.ValidateCode);

            return commitPlayerPhoneService.ExecuteAsync(command);
        }
    }
}
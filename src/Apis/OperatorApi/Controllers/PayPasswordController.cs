using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.PayPassowrd;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/pay/password")]
    [ApiController]
    public class PayPasswordController : ControllerBase
    {
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task SetPayPasswordAsync(
            [FromServices] ICommandService<SetFirstPayPasswordCommand> setPayPasswordService,
            [FromBody] SetPayPasswordRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetFirstPayPasswordCommand(playerTokenData.PlayerCode, request.Password);

            await setPayPasswordService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpPost("validate-code")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask<string> StartChangePayPasswordAsync(
            [FromServices] IGetService<StartChangePayPasswordCommand, string> startChangePayPasswordService)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new StartChangePayPasswordCommand(playerTokenData.PlayerCode, CultureInfo.CurrentUICulture);

            return startChangePayPasswordService.GetAsync(command);
        }

        [HttpPatch]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask ChangePayPasswordAsync(
            [FromServices] ICommandService<ChangePayPasswordCommand> changePayPasswordService,
            ChangePayPasswordRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new ChangePayPasswordCommand(
                playerTokenData.PlayerCode,
                request.NewPassword,
                request.ValidateCode,
                request.SecurityKey);

            return changePayPasswordService.ExecuteAsync(command);
        }
    }
}

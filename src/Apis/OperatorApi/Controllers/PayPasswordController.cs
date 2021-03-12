using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.PayPassowrd;
using Valhalla.Services;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/pay/password")]
    [ApiController]
    public class PayPasswordController : ControllerBase
    {
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task SetPayPasswordAsync(
            [FromServices] ICommandService<SetFirstPayPasswordCommand> setPayPasswordService,
            [FromBody] SetPayPasswordRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetFirstPayPasswordCommand(playerTokenData.PlayerCode, request.Password);

            await setPayPasswordService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}

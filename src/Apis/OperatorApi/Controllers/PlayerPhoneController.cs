using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.PlayerPhone;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerPhoneController : ControllerBase
    {
        /// <summary>
        /// Starts the change phone.
        /// </summary>
        /// <param name="startSetPlayerPhoneService">The start set player phone service.</param>
        /// <param name="request">The request.</param>
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask<string> StartChangePhoneAsync(
            [FromServices] IGetService<StartSetPlayerPhoneCommand, string> startSetPlayerPhoneService,
            [FromBody] StartSetPhoneRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new StartSetPlayerPhoneCommand(playerTokenData.PlayerCode, request.Phone, CultureInfo.CurrentUICulture);

            return startSetPlayerPhoneService.GetAsync(command);
        }
    }
}

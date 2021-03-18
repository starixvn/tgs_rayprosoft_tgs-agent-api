using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.PlayerIdentity;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/player/identity")]
    [ApiController]
    public class PlayerIdentityController : ControllerBase
    {
        [HttpPatch]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask SetPlayerIdentityAsync(
            [FromServices] ICommandService<SetIdentityCommand> setIdentityService,
            [FromBody] SetPlayerIdentityRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetIdentityCommand(playerTokenData.PlayerCode, request.RealName, request.Identity);

            return setIdentityService.ExecuteAsync(command);
        }
    }
}

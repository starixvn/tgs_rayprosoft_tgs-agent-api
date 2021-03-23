using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.IdentityVerifiers;
using TGS.Services.Operations;
using TGS.Services.Operator.PlayerIdentity;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/player/identity")]
    [ApiController]
    public class PlayerIdentityController : ControllerBase
    {
        /// <summary>
        /// Sets the player identity.
        /// </summary>
        /// <param name="setIdentityService">The set identity service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPatch]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask SetPlayerIdentityAsync(
            [FromServices] ICommandService<SetIdentityCommand> setIdentityService,
            [FromBody] SetPlayerIdentityRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetIdentityCommand(playerTokenData.PlayerCode, request.RealName, request.Identity);

            return setIdentityService.ExecuteAsync(command);
        }

        /// <summary>
        /// Checks the identity is valid.
        /// </summary>
        /// <param name="identityVerifierSelector">The identity verifier selector.</param>
        /// <param name="playerOperations">The player operations.</param>
        /// <param name="id">The identify number.</param>
        /// <returns></returns>
        [HttpGet("check/{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<bool> CheckIdentityIsValidAsync(
            [FromServices] IIdentityVerifierSelector<string> identityVerifierSelector,
            [FromServices] IPlayerOperations playerOperations,
            string id)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var player = await playerOperations
                .GetPlayerByCodeAsync(playerTokenData.PlayerCode)
                .ConfigureAwait(false);

            var identityVerifier = identityVerifierSelector.GetIdentityVerifier(player.DefaultWalletCurrencyCode);

            return identityVerifier.Verify(id);
        }
    }
}

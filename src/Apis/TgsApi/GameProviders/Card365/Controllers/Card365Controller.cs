using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.Card365.Services;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Card365.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Controllers
{
    [Route("api/365")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Card365)]
    public class Card365Controller : ControllerBase
    {
        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<ErrorResponse> Entrance([FromForm] EntranceRequest request)
            => ValueTask.FromResult(new ErrorResponse());

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("balance")]
        public async ValueTask<BalanceResponse> PlayersBalanceAsync(
            [FromServices] IGamePlayActions actions,
            [FromServices] ICard365ActionMapper map,
            GetBalanceRequest request)
        {
            var playerCode = request.PlayerCode;
            var r = await actions.GetBalanceAsync(Consts.GameProvider.Card365, playerCode).ConfigureAwait(false);
            return map.Response(r);
        }
    }
}
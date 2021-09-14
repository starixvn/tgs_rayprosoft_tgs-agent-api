using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Card365.Services;
using TGS.Core.Models;
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
        public async ValueTask<BalanceResponse> GetBalanceAsync(
            [FromServices] ICard365BetActionServices services,
            [FromServices] ICard365ActionModelMapper map,
            GetBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return map.Response(r);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("bet")]
        public async ValueTask<BalanceResponse> LockAmountAsync(
            [FromServices] ICard365BetActionServices services,
            [FromServices] ICard365ActionModelMapper map,
            BetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            var r = await services.LockAmountAsync(playerCode, request).ConfigureAwait(false);
            return map.Response(r);
        }
    }
}
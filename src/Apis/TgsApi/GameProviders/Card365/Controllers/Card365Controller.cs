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
        public ValueTask<BalanceResponse> GetBalanceAsync(
            [FromServices] ICard365BetActionServices services,
            GetBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            return services.GetBalanceAsync(playerCode);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("bet")]
        public ValueTask<BalanceResponse> LockAmountAsync(
            [FromServices] ICard365BetActionServices services,
            BetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            return services.LockAmountAsync(playerCode, request);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("cancelbet")]
        public ValueTask<BalanceResponse> CancelLockAmountAsync(
            [FromServices] ICard365BetActionServices services,
            CancelBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            return services.CancelLockAmountAsync(playerCode, request);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("settle")]
        public ValueTask<BalanceResponse> PlaceBetAndSettleAsync(
            [FromServices] ICard365BetActionServices services,
            SettleBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            return services.PlaceBetAndSettleAsync(playerCode, request);
        }
    }
}
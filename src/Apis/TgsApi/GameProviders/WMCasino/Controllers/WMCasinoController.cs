using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Controllers
{
    [Route("api/wm")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.WMCasino)]
    public class WMCasinoController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.WMCasino;

        private readonly IGamePlayActions _actions;
        private readonly IWMCasinoActionModelMapper _map;

        public WMCasinoController(
            IGamePlayActions actions,
            IWMCasinoActionModelMapper map)
        {
            _actions = actions;
            _map = map;
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<ErrorResponse> Entrance([FromForm] BaseRequest request)
            => ValueTask.FromResult(new ErrorResponse());

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("getbalance")]
        public async ValueTask<GetBalanceResponse> PlayersBalanceAsync(GetBalanceRequest request)
        {
            var r = await _actions.GetBalanceAsync(GameProviderCode, request.PlayerCode);
            return _map.Response(request.PlayerCode, r);
        }
    }
}
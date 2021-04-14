using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Saba)]
    public class SabaGameController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Saba;

        private readonly IGamePlayActions _actions;
        private readonly ISabaActionModelMapper _map;

        public SabaGameController(
            IGamePlayActions actions,
            ISabaActionModelMapper map)
        {
            _actions = actions;
            _map = map;
        }

        [HttpPost]
        [Route("getbalance")]
        public async ValueTask<GetBalanceResponse> GetPlayerBalanceAsync(GetBalanceRequest request)
        {
            var playerCode = request.Body.PlayerCode;
            var r = await _actions.GetBalanceAsync(GameProviderCode, request.Body.PlayerCode)
                .ConfigureAwait(false);

            return _map.Response(playerCode, r);
        }
    }
}
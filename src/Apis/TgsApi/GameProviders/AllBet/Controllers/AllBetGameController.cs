using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.AllBet.Services;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings.AllBet;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.AllBet.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Controllers
{
    [Route("api/ab")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.AllBet)]
    public class AllBetGameController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly IAllBetActionModelMapper _map;
        private readonly IAllBetServices _services;

        public AllBetGameController(
            IGamePlayActions actions,
            IAllBetActionModelMapper map,
            IAllBetServices services)
        {
            _actions = actions;
            _map = map;
            _services = services;
        }

        [HttpGet]
        [Route("GetBalance/{playerId}")]
        public async ValueTask<BalanceResponse> GetBalanceAsync([FromRoute] string playerId)
        {
            var agent = HttpContext.Items.Get(AllBetConsts.ContextItems.Agent, default(AllBetAgent));

            var playerCode = _services.GetPlayerCode(playerId, agent.Suffix);
            var result = await _actions.GetBalanceAsync(Consts.GameProvider.AllBet, playerCode);

            return _map.Response(result);
        }
    }
}
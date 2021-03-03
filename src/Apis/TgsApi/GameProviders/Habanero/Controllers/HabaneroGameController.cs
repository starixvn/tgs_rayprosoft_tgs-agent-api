using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Habanero.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Habanero)]
    public sealed class HabaneroGameController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;
        private readonly IGamePlayActions _actions;
        private readonly IHabaneroGameControllerMapper _map;

        public HabaneroGameController(
            IGamePlayActions actions,
            IHabaneroGameControllerMapper map)
        {
            _actions = actions;
            _map = map;
        }

        [HttpPost]
        [Route("authorize")]
        public async Task<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.Detail.GameLaunch)
            {
                var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

                var validationResult = await _actions.ValidatePlayerAsync(_map.Request(playerCode, platformType));
                return _map.Response(playerCode, validationResult);
            }

            var balanceResult = await _actions.GetBalanceAsync(GameProviderCode, playerCode);
            return _map.Response(playerCode, balanceResult);
        }
    }
}
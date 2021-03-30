using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.PragmaticPlay)]
    public sealed class PragmaticPlayGameController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly IPragmaticPlayActionModelMapper _map;

        public PragmaticPlayGameController(
            IGamePlayActions actions,
            IPragmaticPlayActionModelMapper map)
        {
            _actions = actions;
            _map = map;
        }

        [HttpPost]
        [Route("authenticate")]
        public async ValueTask<AuthorizePlayerResponse> AuthorizePlayerAsync([FromForm] AuthorizePlayerRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var validationData = _map.Request(playerCode, platformType);
            var validationResult = await _actions.ValidatePlayerAsync(validationData)
                .ConfigureAwait(false);

            return _map.Response(playerCode, request.Token, validationResult);
        }
    }
}
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Services.GameOperations;

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
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;

        private readonly IGamePlayActions _actions;

        public SabaGameController(
            IGamePlayActions actions)
        {
            _actions = actions;
        }

        //[HttpPost]
        //[Route("balance")]
        //public async ValueTask<GetBalanceResponse> GetPlayerBalanceAsync([FromForm] GetBalanceRequest request)
        //{
        //    var r = await _actions.GetBalanceAsync(GameProviderCode, request.UserId)
        //        .ConfigureAwait(false);

        //    return _map.Response(r);
        //}
    }
}
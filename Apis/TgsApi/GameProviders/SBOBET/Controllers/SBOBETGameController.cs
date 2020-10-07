using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers
{
    [FeatureGate(GameProviderConst.SBOBET)]
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class SBOBETGameController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly ISBOGameControllerMapper _map;

        public SBOBETGameController(
            IGamePlayActions actions,
            ISBOGameControllerMapper map)
        {
            _actions = actions;
            _map = map;
        }

        [Route("GetBalance")]
        public async Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var r = await _actions.GetBalanceAsync(GameProviderConst.SBOBET, request.PlayerCode);
            return _map.Response(request.PlayerCode, r);            
        }
    }
}

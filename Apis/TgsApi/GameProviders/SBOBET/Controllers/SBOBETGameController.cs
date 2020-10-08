using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using ContextItems = TGS.Core.Models.Consts.ContextItems;
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

        [Route("Deduct")]
        public async Task<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        { 
            var items = HttpContext.Items;

            var gameProviderId = (Guid) items[ContextItems.GameProviderId];
            var gameProviderConfigurationId = (Guid)items[ContextItems.GameProviderConfigurationId];
            var platformType = (PlatformType) items[ContextItems.PlatformType];

            var betData = _map.Request(gameProviderId, gameProviderConfigurationId, platformType, request);
            var r = await _actions.PlaceBetAsync(betData);
            return _map.Response(request, r);
        }
    }
}

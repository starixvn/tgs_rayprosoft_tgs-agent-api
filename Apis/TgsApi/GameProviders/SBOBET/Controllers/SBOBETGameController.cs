using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using ContextItems = TGS.Core.Models.Consts.ContextItems;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers
{
    [Route("api")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(GameProviderConst.SBOBET)]
    public sealed class SBOBETGameController : ControllerBase
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGamePlayActions _actions;
        private readonly ISBOGameControllerMapper _map;

        public SBOBETGameController(
            IGameProviderOperations gameProviderOperations,
            IGamePlayActions actions,
            ISBOGameControllerMapper map)
        {
            _gameProviderOperations = gameProviderOperations;
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
            var data = await GetGameProviderConfigurationDataAsync();

            var betData = _map.Request(data, request);
            var r = await _actions.PlaceBetAsync(betData);
            return _map.Response(request, r);
        }

        [Route("Settle")]
        public async Task<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var data = await GetGameProviderConfigurationDataAsync();

            var roundId = request.ExternalTransactionId;
            var betData = _map.Request(data, roundId, request);
            var r = await _actions.SettleBetAsync(betData);
            return _map.Response(request, r);
        }

        private async Task<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync()
        {
            var items = HttpContext.Items;

            var gameProviderId = (Guid)items[ContextItems.GameProviderId];
            var gameProviderConfigurationId = (Guid)items[ContextItems.GameProviderConfigurationId];
            var platformType = (PlatformType)items[ContextItems.PlatformType];

            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = cachedGameProvider.AutoCloseRound,
                PlatformType = platformType
            };
        }
    }
}
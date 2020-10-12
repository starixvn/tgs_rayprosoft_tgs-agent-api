using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameModels;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
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
        private readonly IGamePlayActions _actions;
        private readonly ISBOGameControllerMapper _map;
        private readonly ISBOServices _services;

        public SBOBETGameController(
            IGamePlayActions actions,
            ISBOGameControllerMapper map,
            ISBOServices services)
        {
            _actions = actions;
            _map = map;
            _services = services;
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
            var data = await _services.GetGameProviderConfigurationDataAsync();

            var betData = _map.Request(data, request);
            var r = await _actions.PlaceBetAsync(betData);
            return _map.Response(request, r);
        }

        [Route("Settle")]
        public async Task<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();

            var roundId = request.ExternalTransactionId;
            var betData = _map.Request(data, roundId, request);
            var r = await _actions.SettleBetAsync(betData);
            return _map.Response(request, r);
        }

        [Route("Cancel")]
        public async Task<CancelBetResponse> CancelBetOrVoidRoundAsync(CancelBetRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();
            Func<GameProviderConfigurationData, CancelBetRequest, Task<CancelBetResponse>> func =
                request.IsCancelAll ? VoidRoundAsync : CancelBetAsync;

            return await func(data, request);
        }

        #region Private Methods

        private async Task<CancelBetResponse> CancelBetAsync(GameProviderConfigurationData data, CancelBetRequest request)
        {
            var betData = _map.Request(data, request);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }

        private async Task<CancelBetResponse> VoidRoundAsync(GameProviderConfigurationData data, CancelBetRequest request)
        {
            var gameActionExternalIds = await _services.GetValidGameActionExternalIdsAsync(data.GameProviderId, request.PlayerCode, request.ExternalRoundId);

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }

        #endregion Private Methods
    }
}
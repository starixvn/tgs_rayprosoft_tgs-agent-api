using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.AllBet.Services;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings.AllBet;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.AllBet.Enums;
using TGS.Shared.Game.Models.AllBet.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Controllers
{
    [Route("api/ab")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.AllBet)]
    public class AllBetGameController : ControllerBase
    {
        private readonly
            IReadOnlyDictionary<TransferType,
                Func<string, GameProviderConfigurationData, TransferRequest, ValueTask<BetResult>>> _actionMap;

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

            _actionMap =
                new Dictionary<TransferType,
                    Func<string, GameProviderConfigurationData, TransferRequest, ValueTask<BetResult>>>
                {
                    [TransferType.PlaceBet] = PlaceBetAsync,
                    [TransferType.Settle] = SettleAsync,
                    [TransferType.ManuelSettle] = ManuelSettleAsync
                };
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

        [HttpPost]
        [Route("Transfer")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async ValueTask<BalanceResponse> TransferAsync(TransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);

            if (!_actionMap.TryGetValue(request.Type, out var func))
            {
                throw new InvalidOperationException(
                    $"no matched implementation of transfer type '{(int)request.Type}'");
            }

            var result = await func(playerCode, data, request);

            return _map.Response(result);
        }

        /// <summary>
        ///     Use to cancel place bet
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelTransfer")]
        [Consumes(MediaTypeNames.Application.Json)]
        public ValueTask<BalanceResponse> CancelTransferAsync(CancelTransferRequest request)
            => new ValueTask<BalanceResponse>();


        private async ValueTask<BetResult> PlaceBetAsync(string playerCode, GameProviderConfigurationData data,
            TransferRequest request)
        {
            var betData = _map.BetRequest(playerCode, data, request);

            var result = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            return result;
        }

        private ValueTask<BetResult> SettleAsync(string playerCode, GameProviderConfigurationData data,
            TransferRequest request) => ValueTask.FromResult(new BetResult());

        private ValueTask<BetResult> ManuelSettleAsync(string playerCode, GameProviderConfigurationData data,
            TransferRequest request) => throw new NotImplementedException();
    }
}
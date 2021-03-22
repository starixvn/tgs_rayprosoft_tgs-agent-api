using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Habanero.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Habanero.Enums;
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

        private readonly IReadOnlyDictionary<ActionType, Func<BetData, Task<BetResult>>> _actionMapper;
        private readonly IGamePlayActions _actions;
        private readonly IHabaneroActionModelMapper _map;
        private readonly IHabaneroServices _services;
        private readonly ISettingOperations _settingOperations;

        public HabaneroGameController(
            IGamePlayActions actions,
            IHabaneroServices services,
            IHabaneroActionModelMapper map,
            ISettingOperations settingOperations)
        {
            _actions = actions;
            _services = services;
            _map = map;
            _settingOperations = settingOperations;

            _actionMapper = new Dictionary<ActionType, Func<BetData, Task<BetResult>>>
            {
                {ActionType.PlaceBet, actions.PlaceBetAsync},
                {ActionType.SettleBet, actions.SettleBetAsync},
                {ActionType.FreeBet, actions.SettleBetAsync},
                {ActionType.CancelTransaction, actions.CancelTransactionAsync},
                {ActionType.PlaceAndSettleBet, actions.PlaceAndSettleBetAsync},
                {ActionType.RetrySettlement, actions.SettleBetAsync}
            };
        }

        [HttpPost]
        [Route("authorize")]
        public async ValueTask<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.Detail.GameLaunch)
            {
                var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

                var validationData = _map.Request(playerCode, platformType);
                var validationResult = await _actions.ValidatePlayerAsync(validationData)
                    .ConfigureAwait(false);

                return _map.Response(playerCode, validationResult);
            }

            var balanceResult = await _actions.GetBalanceAsync(GameProviderCode, playerCode).ConfigureAwait(false);
            return _map.Response(playerCode, balanceResult);
        }

        [HttpPost]
        [Route("transfer")]
        public async ValueTask<FundTransferResponse> TransferFundsAsync(FundTransferRequest request)
        {
            var actionType = request.Detail.ActionType;
            Request.Path = new($"{Request.Path}/{actionType.ToString().ToLower()}");

            if (actionType is ActionType.PlaceBet or ActionType.PlaceAndSettleBet)
            {
                var configuration =
                    await _settingOperations.GetValueFromJsonByKeyAsync<HabaneroConfiguration>(Consts.GameProvider
                        .Habanero);

                if (configuration.DelaySecondsForPlaceBet > 0)
                {
                    await Task.Delay(configuration.DelaySecondsForPlaceBet * 1000).ConfigureAwait(false);
                }
            }

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.Request(playerCode, data, request);
            var execute = _actionMapper[actionType];

            var r = await execute(betData).ConfigureAwait(false);

            return _map.Response(request, r);
        }

        [HttpPost]
        [Route("transfer/query")]
        public async ValueTask<QueryResponse> QueryAsync(QueryRequest request)
        {
            var exists = await _services.GameActionExistsAsync(request.Detail)
                .ConfigureAwait(false);

            return _map.Response(exists);
        }
    }
}
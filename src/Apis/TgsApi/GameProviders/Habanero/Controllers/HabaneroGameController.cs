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
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
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

        [HttpPost]
        [Route("authorize")]
        public async Task<AuthorizePlayerResponse> AuthorizePlayerAsync(
            [FromServices] IGamePlayActions actions,
            [FromServices] IHabaneroActionModelMapper map,
            AuthorizePlayerRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (request.Detail.GameLaunch)
            {
                var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

                var validationResult = await actions.ValidatePlayerAsync(map.Request(playerCode, platformType));
                return map.Response(playerCode, validationResult);
            }

            var balanceResult = await actions.GetBalanceAsync(GameProviderCode, playerCode);
            return map.Response(playerCode, balanceResult);
        }

        [HttpPost]
        [Route("transfer")]
        public async Task<FundTransferResponse> TransferFundsAsync(
            [FromServices] IGamePlayActions actions,
            [FromServices] IHabaneroServices services,
            [FromServices] IHabaneroActionModelMapper map,
            FundTransferRequest request)
        {
            var actionType = request.Detail.ActionType;
            Request.Path = new($"{Request.Path}/{actionType.ToString().ToLower()}");

            var actionMapper = new Dictionary<ActionType, Func<BetData, Task<BetResult>>>
            {
                {ActionType.PlaceBet, actions.PlaceBetAsync},
                {ActionType.SettleBet, actions.SettleBetAsync},
                {ActionType.FreeBet, actions.SettleBetAsync},
                {ActionType.CancelTransaction, actions.CancelTransactionAsync},
                {ActionType.PlaceAndSettleBet, actions.PlaceAndSettleBetAsync},
                {ActionType.RetrySettlement, actions.SettleBetAsync}
            };

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await services.GetGameProviderConfigurationDataAsync();

            var betData = map.Request(playerCode, data, request);
            var execute = actionMapper[actionType];

            var r = await execute(betData);

            return map.Response(request, r);
        }
    }
}
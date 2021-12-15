using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Jili.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Controllers
{
    [Route("api/jili")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Jili)]
    public class JiliController : ControllerBase
    {
        private readonly ILogger<JiliController> _logger;

        public JiliController(
            ILogger<JiliController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("auth")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async ValueTask<JiliAuthorizePlayerResponse> AuthorizePlayerAsync(
            [FromServices] IGamePlayActions actions,
            [FromServices] IJiliActionModelMapper map,
            JiliAuthorizePlayerRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var data = map.ValidateRequest(playerCode, platformType);
            var result = await actions.ValidatePlayerAsync(data).ConfigureAwait(false);

            return map.AuthorizePlayerResponse(playerCode, result);
        }

        [Route("bet")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async ValueTask<JiliPlaceBetAndSettleResponse> AuthorizePlayerAsync(
            [FromServices] IGamePlayActions actions,
            [FromServices] IJiliService service,
            [FromServices] IJiliActionModelMapper map,
            JiliPlaceBetAndSettleRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var jiliPlayerId = items.Get(JiliConsts.ContextItems.JiliPlayerId, string.Empty);

            var data = await service.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);

            BetResult result;

            if (request.IsFreeRound is true)
            {
                var betData = map.FreeBetRequest(playerCode, data, request);
                result = await actions.SettleBetAsync(betData).ConfigureAwait(false);
            }
            else
            {
                var betData = map.BetAndSettleRequest(playerCode, data, request);
                result = await actions.PlaceAndSettleBetAsync(betData).ConfigureAwait(false);
            }

            return map.BetAndSettleResponse(jiliPlayerId, result);
        }

        [Route("cancelBet")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async ValueTask<JiliCancelTransactionResponse> CancelTransactionAsync(
            [FromServices] IGamePlayActions actions,
            [FromServices] IJiliService service,
            [FromServices] IJiliActionModelMapper map,
            [FromServices] IGameActionQueries gameActionQueries,
            JiliCancelTransactionRequest request)
        {
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var jiliPlayerId = items.Get(JiliConsts.ContextItems.JiliPlayerId, string.Empty);

            var externalRoundId = request.ExternalRoundId.ToString();
            var externalGameId = request.ExternalGameId.ToString();

            var balanceResult = await actions.GetBalanceAsync(Consts.GameProvider.Jili, playerCode)
                .ConfigureAwait(false);

            var gameActionData = await service
                .GetGameActionsByExternalRoundIdAsync(
                    gameProviderId,
                    playerCode,
                    externalRoundId,
                    externalGameId)
                .ConfigureAwait(false);

            if (gameActionData == null)
            {
                // TODO: add pending game action to redis

                return map.FailCancelTransactionResponse(2, "Round Not Found", jiliPlayerId, balanceResult);
            }

            var cancelledExternalTransactionId = $"{externalRoundId}_cancel";
            var gameActionCancelled = gameActionData.GameActions.FirstOrDefault(ga
                => ga.ExternalTransactionId == cancelledExternalTransactionId);

            if (gameActionCancelled != null)
            {
                return map.SuccessCancelTransactionResponse(jiliPlayerId, balanceResult, gameActionCancelled.Id);
            }

            return map.FailCancelTransactionResponse(6, "Already accepted and can't be canceled",
                jiliPlayerId, balanceResult);
        }
    }
}

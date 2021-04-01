using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
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
        private static readonly string GameProviderCode = Consts.GameProvider.PragmaticPlay;

        private readonly IGamePlayActions _actions;
        private readonly IPragmaticPlayActionModelMapper _map;
        private readonly IPragmaticPlayServices _services;

        public PragmaticPlayGameController(
            IGamePlayActions actions,
            IPragmaticPlayServices services,
            IPragmaticPlayActionModelMapper map)
        {
            _actions = actions;
            _services = services;
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

        [HttpPost]
        [Route("balance")]
        public async ValueTask<GetBalanceResponse> GetPlayerBalanceAsync([FromForm] GetBalanceRequest request)
        {
            var r = await _actions.GetBalanceAsync(GameProviderCode, request.UserId)
                .ConfigureAwait(false);

            return _map.Response(r);
        }

        [HttpPost]
        [Route("bet")]
        public async ValueTask<PlaceBetResponse> PlaceBetAsync([FromForm] PlaceBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.Request(playerCode, data, request);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            var externalRoundId = request.RoundId;
            var transaction = r.Transactions.First();
            await _services.CacheGameActionExternalIdAsync(data.GameProviderId, playerCode,
                externalRoundId, transaction.GameId, transaction.ExternalTransactionId);

            return _map.BetResponse(r);
        }

        [HttpPost]
        [Route("result")]
        public async Task<ResultResponse> SettleBetAsync([FromForm] ResultRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var externalRoundId = request.RoundId;
            var externalGameId = request.GameId;
            var gameActionExternalId = await _services
                .GetGameActionExternalIdAsync(data.GameProviderId, playerCode, data.PlatformType, externalRoundId,
                    externalGameId, externalRoundId)
                .ConfigureAwait(false);

            if (gameActionExternalId.IsNullOrEmpty())
            {
                throw new InvalidGameActionException("Referenced transaction is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var betData = _map.Request(playerCode, data, request, gameActionExternalId);
            var r = await _actions.SettleBetAsync(betData).ConfigureAwait(false);

            var transaction = r.Transactions.First();
            await _services.CacheRoundStatusAsync(data.GameProviderId, playerCode, externalRoundId, transaction.GameId);

            return _map.ResultResponse(r);
        }

        [HttpPost]
        [Route("endround")]
        public async Task<EndRoundResponse> EndRoundAsync([FromForm] EndRoundRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var externalRoundId = request.RoundId;
            var externalGameId = request.GameId;
            var status = await _services.GetRoundStatusAsync(data.GameProviderId, playerCode, data.PlatformType,
                    externalRoundId, externalGameId)
                .ConfigureAwait(false);

            if (status == RoundStatus.Closed)
            {
                var balanceResult = await _actions.GetBalanceAsync(GameProviderCode, request.UserId)
                    .ConfigureAwait(false);

                return _map.EndRoundResponse(balanceResult.Balance);
            }

            var gameActionExternalId = await _services
                .GetGameActionExternalIdAsync(data.GameProviderId, playerCode, data.PlatformType, externalRoundId,
                    externalGameId, externalRoundId)
                .ConfigureAwait(false);

            if (gameActionExternalId.IsNullOrEmpty())
            {
                throw new InvalidGameActionException("Referenced transaction is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var betData = _map.Request(playerCode, data, request, gameActionExternalId);
            var betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
            return _map.EndRoundResponse(betResult.Balance);
        }

        [HttpPost]
        [Route("refund")]
        public async Task<RefundResponse> CancelTransactionAsync([FromForm] RefundRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.Request(playerCode, data, request);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            return _map.RefundResponse(r);
        }
    }
}
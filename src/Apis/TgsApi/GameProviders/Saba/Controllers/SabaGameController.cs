using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Saba.Attributes;
using TGS.Apis.TgsApi.GameProviders.Saba.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Controllers
{
    [Route("api/saba")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Saba)]
    public class SabaGameController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Saba;

        private readonly IGamePlayActions _actions;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISabaActionModelMapper _map;
        private readonly ISabaServices _services;

        public SabaGameController(
            IGamePlayActions actions,
            IGameProviderOperations gameProviderOperations,
            ISabaActionModelMapper map,
            ISabaServices services)
        {
            _actions = actions;
            _gameProviderOperations = gameProviderOperations;
            _map = map;
            _services = services;
        }

        [HttpPost]
        [Route("getbalance")]
        [ResolveRequest(Type = typeof(GetBalanceRequest))]
        public async ValueTask<GetBalanceResponse> GetPlayerBalanceAsync(GetBalanceRequest request)
        {
            var playerCode = request.Body.PlayerCode;
            var r = await _actions.GetBalanceAsync(GameProviderCode, request.Body.PlayerCode)
                .ConfigureAwait(false);

            return _map.Response(playerCode, r);
        }

        [HttpPost]
        [Route("placebet")]
        [ResolveRequest(Type = typeof(PlaceBetRequest))]
        public async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var betData = _map.Request(playerCode, data, request);
            var r = await _actions.LockAmountAsync(betData).ConfigureAwait(false);

            return _map.Response(playerCode, request.Body, r);
        }

        [HttpPost]
        [Route("confirmbet")]
        [ResolveRequest(Type = typeof(ConfirmBetRequest))]
        public async ValueTask<ConfirmBetResponse> ConfirmBetAsync(ConfirmBetRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            decimal unlockAmount;
            var confirmBet = request.Body.Transactions[0];

            if (confirmBet.IsOddsChanged)
            {
                var gameProviderId = data.GameProviderId;
                var game = (await _gameProviderOperations
                    .GetCachedGamesAsync(gameProviderId, SabaConsts.ExternalGameId)
                    .ConfigureAwait(false)).FirstOrDefault();

                var externalRoundId = confirmBet.ExternalRoundId.ToString();
                var gameActionExternalId =
                    (await _services.GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId
                        , game!.Id, new[] {GameActionType.LockAmount}).ConfigureAwait(false))
                    .FirstOrDefault();

                if (gameActionExternalId == null)
                {
                    throw new InvalidGameActionException($"No lock transaction is found in round '{externalRoundId}'",
                        InvalidGameActionErrorCode.TransactionNotFound);
                }

                unlockAmount = gameActionExternalId.Amount;
            }
            else
            {
                unlockAmount = confirmBet.ActualAmount;
            }

            var betData = _map.Request(playerCode, data, request, unlockAmount);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            return _map.Response(playerCode, request.Body, r);
        }
    }
}
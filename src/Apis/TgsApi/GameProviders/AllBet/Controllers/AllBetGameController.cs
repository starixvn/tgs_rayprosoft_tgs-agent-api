using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.AllBet.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.GameProviders.Settings.AllBet;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
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
        private readonly IAllBetCacheServices _cacheServices;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IAllBetActionModelMapper _map;
        private readonly IAllBetServices _services;

        public AllBetGameController(
            IGamePlayActions actions,
            IAllBetActionModelMapper map,
            IAllBetServices services,
            IAllBetCacheServices cacheServices,
            IGameProviderOperations gameProviderOperations)
        {
            _actions = actions;
            _map = map;
            _services = services;
            _cacheServices = cacheServices;
            _gameProviderOperations = gameProviderOperations;

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
        public async ValueTask<BalanceResponse> CancelTransferAsync(CancelTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync().ConfigureAwait(false);

            var groupId = request.ReferencedTransactionId.ToString();
            var externalIds = await _services.GetGameActionExternalIdsAsync(data.GameProviderId, playerCode, groupId)
                .ConfigureAwait(false);

            var betData = _map.CancelBetRequest(playerCode, data, externalIds, request);
            var result = await _actions.CancelTransactionAsync(betData);

            return _map.Response(result);
        }

        private async ValueTask<BetResult> PlaceBetAsync(string playerCode, GameProviderConfigurationData data,
            TransferRequest request)
        {
            await _cacheServices.CachePlayerCodeByExternalGameActionGroupIdAsync(
                request.TransactionId.ToString(), playerCode).ConfigureAwait(false);

            var betData = _map.BetRequest(playerCode, data, request);
            return await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
        }

        /// <summary>
        ///     For settle request, only contains 1 transaction with 1 betNum
        /// </summary>
        private async ValueTask<BetResult> SettleAsync(string playerCode, GameProviderConfigurationData data,
            TransferRequest request)
        {
            var detail = request.Details[0];
            var roundId = detail.RoundId.ToString();
            var externalGameId = detail.GameType.ToString();

            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, externalGameId);
            var gameId = games.FirstOrDefault()?.Id;

            if (!gameId.HasValue)
            {
                throw new InvalidGameActionException($"Game '{externalGameId}' is not found",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            var externalIds = await _services
                .GetValidGameActionExternalIdsAsync(data.GameProviderId, playerCode, roundId, gameId.Value)
                .ConfigureAwait(false);

            CheckIfBetExists(externalIds, detail.BetNumber.ToString());

            var settledBetIds = externalIds.Where(t => t.GameActionType.IsSettleBet())
                .Select(t => t.ExternalBetId).ToArray();

            var forceCloseRound = GetForceCloseRoundFlag(externalIds, settledBetIds);

            var betData = _map.SettleBetRequest(playerCode, data, externalIds, request, forceCloseRound);

            return await _actions.SettleBetAsync(betData).ConfigureAwait(false);
        }


        private async ValueTask<BetResult> ManuelSettleAsync(string playerCode, GameProviderConfigurationData data,
            TransferRequest request)
        {
            var detail = request.Details[0];
            var roundId = detail.RoundId.ToString();
            var externalGameId = detail.GameType.ToString();

            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId, externalGameId);
            var gameId = games.FirstOrDefault()?.Id;

            if (!gameId.HasValue)
            {
                throw new InvalidGameActionException($"Game '{externalGameId}' is not found",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            var externalIds = await _services
                .GetValidGameActionExternalIdsAsync(data.GameProviderId, playerCode, roundId, gameId.Value)
                .ConfigureAwait(false);

            CheckIfBetExists(externalIds, detail.BetNumber.ToString());

            var settledBetIds = externalIds.Where(t => t.GameActionType.IsSettleBet())
                .Select(t => t.ExternalBetId).ToArray();

            if (settledBetIds.Length != 0)
            {
                return detail.Status switch
                {
                    BetStatus.Settled => await ResettleAsync(playerCode, data, externalIds, request)
                        .ConfigureAwait(false),
                    BetStatus.Refund => await VoidRoundAsync(playerCode, data, externalIds, request)
                        .ConfigureAwait(false),
                    _ => throw new InvalidGameActionException(
                        $"Unexpected status '{(int)detail.Status}' of action {(int)request.Type}")
                };
            }

            var forceCloseRound = GetForceCloseRoundFlag(externalIds, settledBetIds);
            var betData = _map.SettleBetRequest(playerCode, data, externalIds, request, forceCloseRound);

            return await _actions.SettleBetAsync(betData).ConfigureAwait(false);
        }

        private ValueTask<BetResult> VoidRoundAsync(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request)
        {
            var betData = _map.VoidRoundRequest(playerCode, data, externalIds, request);
            return _actions.CancelTransactionAsync(betData);
        }

        private ValueTask<BetResult> ResettleAsync(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request)
        {
            var betData = _map.ResettleBetRequest(playerCode, data, externalIds, request);
            return _actions.SettleBetAsync(betData);
        }

        private static void CheckIfBetExists(GameActionExternalIds[] externalIds, string betNumber)
        {
            var betExists = externalIds.Any(t => t.GameActionType == GameActionType.PlaceBet &&
                t.ExternalBetId.IgnoreCaseEquals(betNumber));

            if (!betExists)
            {
                throw new InvalidGameActionException($"Bet with number '{betNumber}' is not found",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }
        }

        private static bool GetForceCloseRoundFlag(GameActionExternalIds[] externalIds, string[] settledBetIds)
        {
            var unsettles = externalIds
                .Where(t => t.GameActionType == GameActionType.PlaceBet &&
                    !settledBetIds.Contains(t.ExternalBetId))
                .Select(t => t.ExternalBetId)
                .Distinct().ToArray();

            return unsettles.Length == 1;
        }
    }
}
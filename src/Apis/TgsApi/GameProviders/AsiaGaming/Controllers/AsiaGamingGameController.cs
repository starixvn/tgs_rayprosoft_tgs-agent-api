using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Controllers
{
    [Route("api/ag")]
    [ApiController]
    [Consumes(MediaTypeNames.Text.Xml, MediaTypeNames.Application.Xml)]
    [Produces(MediaTypeNames.Text.Xml, MediaTypeNames.Application.Xml)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.AsiaGaming)]
    public sealed class AsiaGamingGameController : ControllerBase
    {
        private readonly IReadOnlyDictionary<TransactionType, Func<PostTransferRequest, ValueTask<TransferResponse>>> _actionMapper;
        private readonly IGamePlayActions _actions;
        private readonly IAsiaGamingActionModelMapper _map;
        private readonly IAsiaGamingServices _services;
        private readonly ISettingOperations _settingOperations;

        public AsiaGamingGameController(
            IGamePlayActions actions,
            IAsiaGamingServices services,
            IAsiaGamingActionModelMapper map,
            ISettingOperations settingOperations)
        {
            _actions = actions;
            _services = services;
            _map = map;
            _settingOperations = settingOperations;

            _actionMapper = new Dictionary<TransactionType, Func<PostTransferRequest, ValueTask<TransferResponse>>>
            {
                {TransactionType.BET, PlaceBetAsync},
                {TransactionType.WIN, SettleBetAsync},
                {TransactionType.LOSE, SettleBetAsync},
                {TransactionType.REFUND, CancelTransactionAsync}
            };
        }

        [HttpPost("betresponse")]
        public async ValueTask<TransferResponse> BetResponseAsync(
            [FromServices] IAsiaGamingDataStore store,
            BetResponseRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            if (!(request.Records?.Length > 0))
            {
                return new TransferResponse
                {
                    ResponseCode = GameApiError.OK
                };
            }

            var transactionId = request.TransactionId;

            var isExist = await store
                .ExistBetResponseTransactionIdAsync(transactionId)
                .ConfigureAwait(false);

            if (isExist)
            {
                return new TransferResponse
                {
                    ResponseCode = GameApiError.OK
                };
            }

            _ = await store
                .StoreBetResponseTransactionIdAsync(transactionId)
                .ConfigureAwait(false);

            var data = await _services
                .GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var lockGameActionExternalIds =
                await _services.GetLockGameActionExternalIdsAsync(playerCode, transactionId);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.AsiaGaming);

            var betData = _map.PlaceBetForLockGameRequest(playerCode, data, request,
                lockGameActionExternalIds, configuration.OppositeBets);

            _ = await _actions.PlaceBetAsync(betData);

            await store
                .RemoveBetResponseTransactionIdAsync(transactionId)
                .ConfigureAwait(false);

            return new TransferResponse
            {
                ResponseCode = GameApiError.OK
            };
        }

        [HttpPost("posttransfer")]
        public ValueTask<TransferResponse> PostTransferAsync(PostTransferRequest request)
        {
            var transactionType = request.Record.TransactionType;
            Request.Path = new($"{Request.Path}/{transactionType.ToString().ToLower()}");

            return _actionMapper[transactionType](request);
        }

        private async ValueTask<TransferResponse> CancelTransactionAsync(PostTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.AsiaGaming);

            var isLockGame = configuration.LockGameTypes?.IgnoreCaseContains(request.Record.GameType) == true;
            var betData = _map.CancelTransactionRequest(playerCode, data, request, isLockGame, configuration.OppositeBets);

            var r = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);

            return _map.Response(r);
        }

        private async ValueTask<TransferResponse> PlaceBetAsync(PostTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.AsiaGaming);

            var isLockGame = configuration.LockGameTypes?.IgnoreCaseContains(request.Record.GameType) == true;

            BetResult r;

            if (isLockGame)
            {
                var betData = _map.LockAmountRequest(playerCode, data, request);
                r = await _actions.LockAmountAsync(betData).ConfigureAwait(false);
            }
            else
            {
                var betData = _map.PlaceBetRequest(playerCode, data, request, configuration.OppositeBets);
                r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
            }

            return _map.Response(r);
        }

        private async ValueTask<TransferResponse> SettleBetAsync(PostTransferRequest request)
        {
            var transactionType = request.Record.TransactionType;

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.AsiaGaming);

            var isLockGame = configuration.LockGameTypes?.IgnoreCaseContains(request.Record.GameType) == true;

            var betData = _map.SettleBetRequest(playerCode, data, request, transactionType,
                    isLockGame, configuration.OppositeBets);

            var result = await _actions.SettleBetAsync(betData).ConfigureAwait(false);

            return _map.Response(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
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
    [Consumes(MediaTypeNames.Application.Xml)]
    [Produces(MediaTypeNames.Application.Xml)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.AsiaGaming)]
    public sealed class AsiaGamingGameController : ControllerBase
    {
        private readonly IReadOnlyDictionary<TransactionType, Func<BetData, ValueTask<BetResult>>> _actionMapper;
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

            _actionMapper = new Dictionary<TransactionType, Func<BetData, ValueTask<BetResult>>>
            {
                {TransactionType.BET, actions.PlaceBetAsync},
                {TransactionType.WIN, actions.SettleBetAsync},
                {TransactionType.LOSE, actions.SettleBetAsync},
                {TransactionType.REFUND, actions.CancelTransactionAsync}
            };
        }

        [HttpPost("posttransfer")]
        public async ValueTask<TransferResponse> PostTransferAsync(
            [FromServices] ITgsLog log,
            [FromServices] IJsonSerializationProvider json,
            PostTransferRequest request)
        {
            var transactionType = request.Record.TransactionType;
            Request.Path = new($"{Request.Path}/{transactionType.ToString().ToLower()}");

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.AsiaGaming);

            BetData betData;
            var lockGame = configuration.LockGameTypes?.IgnoreCaseContains(request.Record.GameType) == true;

            BetResult r;
            if (lockGame && transactionType == TransactionType.BET)
            {
                betData = _map.LockAmountRequest(playerCode, data, request);
                r = await _actions.LockAmountAsync(betData).ConfigureAwait(false);
            }
            else
            {
                betData = _map.Request(playerCode, data, request, transactionType, 
                    lockGame, configuration.OppositeBets);
                var execute = _actionMapper[transactionType];
                r= await execute(betData).ConfigureAwait(false);
            }

            var options = new JsonSerializerOptions {WriteIndented = false};
            log.LogInfo(
                $"RoundId: {request.Record.RoundId}, BetData: {json.SerializeToString(betData, options)}, BetResult: {json.SerializeToString(r, options)}");

            return _map.Response(r);
        }
        
        [HttpPost("betresponse")]
        public async ValueTask<TransferResponse> BetResponseAsync(
            [FromServices] ITgsLog log,
            [FromServices] IJsonSerializationProvider json,
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
            
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);
            var lockGameActionExternalIds = await _services.GetLockGameActionExternalIdsAsync(playerCode, request.TransactionId);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.AsiaGaming);

            var betData = _map.PlaceBetRequest(playerCode, data, request, 
                lockGameActionExternalIds, configuration.OppositeBets);
            var r = await _actions.PlaceBetAsync(betData);
            
            var options = new JsonSerializerOptions {WriteIndented = false};
            log.LogInfo(
                $"RoundId: {lockGameActionExternalIds.ExternalRoundId}, BetData: {json.SerializeToString(betData, options)}, BetResult: {json.SerializeToString(r, options)}");

            return new TransferResponse
            {
                ResponseCode = GameApiError.OK
            };
        }
    }
}
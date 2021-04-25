using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.JDB.Services;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Controllers
{
    [Route("api/jdb")]
    [ApiController]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.JDB)]
    public sealed class JDBGameController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.JDB;

        private readonly IReadOnlyDictionary<ActionType, Func<BetData, Task<BetResult>>> _actionMapper;
        private readonly IGamePlayActions _actions;
        private readonly IJDBActionModelMapper _map;
        private readonly IJDBServices _services;
        private readonly ISettingOperations _settingOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public JDBGameController(
            IGamePlayActions actions,
            IJDBServices services,
            IJDBActionModelMapper map,
            ISettingOperations settingOperations,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _actions = actions;
            _services = services;
            _map = map;
            _settingOperations = settingOperations;
            _jsonSerializationProvider = jsonSerializationProvider;

            _actionMapper = new Dictionary<ActionType, Func<BetData, Task<BetResult>>>
            {
                {ActionType.PlaceAndSettleBet, actions.PlaceAndSettleBetAsync},
                {ActionType.CancelTransaction, actions.CancelTransactionAsync}
            };
        }

        [HttpPost]
        [Route("transfer")]
        public Task<BaseResponse> TransferFundsAsync([FromForm] string x)
        {
            var baseRequest = _jsonSerializationProvider.DeserializeFromString<BaseActionRequest>(x);

            switch (baseRequest.ActionType)
            {
                case ActionType.GetBalance:
                    return GetBalanceAsync(_jsonSerializationProvider.DeserializeFromString<GetBalanceRequest>(x));
                case ActionType.PlaceAndSettleBet:
                    return PlaceAndSettleBetAsync(
                        _jsonSerializationProvider.DeserializeFromString<PlaceAndSettleBetRequest>(x));
                case ActionType.CancelTransaction:
                    return CancelTransactionAsync(
                        _jsonSerializationProvider.DeserializeFromString<CancelTransactionRequest>(x));
                default:
                    return Task.FromResult(new BaseResponse {Status = "0000", ErrorMessage = "Invalid action"});
            }
        }

        private async Task<BaseResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var result = await _actions.GetBalanceAsync(Consts.GameProvider.JDB, request.UserId)
                .ConfigureAwait(false);
            
            return _map.Response(request.UserId, result);
        }

        private async Task<BaseResponse> PlaceAndSettleBetAsync(PlaceAndSettleBetRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync()
                .ConfigureAwait(false);
            var betData = _map.Request(data, request);

            var betResult = await _actions.PlaceAndSettleBetAsync(betData).ConfigureAwait(false);
            return _map.Response(request.UserId, betResult);
        }

        private async Task<BaseResponse> CancelTransactionAsync(CancelTransactionRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();

            var betData = _map.Request(data, request);
            var betResult = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(request.UserId, betResult);
        }

    }
}
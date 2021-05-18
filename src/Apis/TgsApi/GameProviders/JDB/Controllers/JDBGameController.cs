using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.JDB.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Controllers
{
    [Route("api/jdb/entrance")]
    [ApiController]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.JDB)]
    public sealed class JDBGameController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly IJDBActionModelMapper _map;
        private readonly IJDBServices _services;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGameActionQueries _gameActionQueries;

        public JDBGameController(
            IGamePlayActions actions,
            IJDBServices services,
            IJDBActionModelMapper map,
            IJsonSerializationProvider jsonSerializationProvider,
            IGameActionQueries gameActionQueries)
        {
            _actions = actions;
            _services = services;
            _map = map;
            _jsonSerializationProvider = jsonSerializationProvider;
            _gameActionQueries = gameActionQueries;
        }

        [HttpPost]
        public Task<BaseActionResponse> TransferFundsAsync([FromForm] string x)
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
                    return Task.FromResult(new BaseActionResponse {Status = GameApiStatus.Succeed.Description(), ErrorMessage = "Invalid action"});
            }
        }

        private async Task<BaseActionResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var result = await _actions.GetBalanceAsync(Consts.GameProvider.JDB, request.UserId)
                .ConfigureAwait(false);
            
            return _map.Response(result);
        }

        private async Task<BaseActionResponse> PlaceAndSettleBetAsync(PlaceAndSettleBetRequest request)
        {
            var platformType = "Web".IgnoreCaseEquals(request.ClientType) ? PlatformType.Desktop : PlatformType.Mobile;
            var data = await _services.GetGameProviderConfigurationDataAsync(platformType)
                .ConfigureAwait(false);
            var betData = _map.Request(data, request, platformType);

            var betResult = await _actions.PlaceAndSettleBetAsync(betData).ConfigureAwait(false);
            return _map.Response(betResult);
        }

        private async Task<BaseActionResponse> CancelTransactionAsync(CancelTransactionRequest request)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync(PlatformType.Desktop);
            var gameActionExternalIds = await _gameActionQueries.GetGameActionExternalIdAsync(data.GameProviderId, request.UserId,
                request.TransferId.ToString());

            var betData = _map.Request(data, request, gameActionExternalIds.ExternalRoundId);
            var betResult = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(betResult);
        }

    }
}
using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Controllers
{
    [Route("api/bg")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.BigGaming)]
    public sealed class BigGamingController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBigGamingModelActionMapper _map;
        private readonly IBigGamingServices _services;
        public BigGamingController(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBigGamingModelActionMapper map,
            IGamePlayActions actions,
            IBigGamingServices services

            )
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        [HttpPost("entrance")]
        public async Task<BGBaseResponse> EntranceAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            BGBaseRequest baseRequest)
        {

            return baseRequest.Method.ToLower() switch
            {
                "open.operator.user.balance" => await BalanceAsync(baseRequest),
                "open.operator.order.transfer" => await BetAsync(jsonOptionsAccessor, baseRequest),
                "open.operator.calc.transfer" => await SettleAsync(jsonOptionsAccessor, baseRequest),
                "open.operator.order.status" => await OrderStatusAsync(jsonOptionsAccessor, baseRequest),
                _ => new BGBaseResponse()
                {
                    Id = baseRequest.Id,
                    Result = null,
                    Error = null,
                    Jsonrpc = baseRequest.Jsonrpc
                }
            };
        }

        [HttpPost("balance")]
        public async Task<BGBaseResponse> BalanceAsync(BGBaseRequest baseRequest)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var balance = await _services
             .GetPlayerBalanceAsync(playerCode)
             .ConfigureAwait(false);

            return new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(balance)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("bet")]
        public async Task<BGBaseResponse> BetAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            BGBaseRequest baseRequest)
        {
            var data = JsonSerializer.Deserialize<BGBetRequest>(JsonSerializer.Serialize(baseRequest.Params), jsonOptionsAccessor.Value.JsonSerializerOptions);
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
             .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
             .ConfigureAwait(false);


            var betData = _map.Request(gameProviderConfigurationData, data);
            var betResult = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);


            var result = new BGBetResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = betResult.Balance,
                OrderResult = "1",
                TranId = data.TranId
            };

            return new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("settle")]
        public async Task<BGBaseResponse> SettleAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            BGBaseRequest baseRequest)
        {
            var data = JsonSerializer.Deserialize<BGSettleRequest>(JsonSerializer.Serialize(baseRequest.Params), jsonOptionsAccessor.Value.JsonSerializerOptions);
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
           .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
           .ConfigureAwait(false);


            decimal balance;
            if (data.Recalc == "1")
            {
                balance = await _map.ResettleAsync(gameProviderConfigurationData, data, playerCode).ConfigureAwait(false);
            }
            else
            {
                balance = await _map.SettleAsync(gameProviderConfigurationData, data, playerCode).ConfigureAwait(false);

            }

            var result = new BGSettleResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = balance,
                OrderResult = "1",
                TranId = data.TranId
            };

            return new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("orderstatus")]
        public async Task<BGBaseResponse> OrderStatusAsync(
               [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
               BGBaseRequest baseRequest)
        {
            var data = JsonSerializer.Deserialize<BGOrderStatusRequest>(JsonSerializer.Serialize(baseRequest.Params), jsonOptionsAccessor.Value.JsonSerializerOptions);
            var result = new BGOrderStatusResponse()
            {
                Result = null
            };

            return new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

    }
}

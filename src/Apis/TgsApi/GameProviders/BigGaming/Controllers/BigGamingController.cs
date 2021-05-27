using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Enums;
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
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IBigGamingModelActionMapper _map;
        private readonly IReadOnlyDictionary<string, Func<BigGamingRequest, Task<BigGamingResponse>>> _methodMapper;
        private readonly IBigGamingServices _services;
        private readonly ISettingOperations _settingOperations;

        public BigGamingController(
            IBigGamingModelActionMapper map,
            IGamePlayActions actions,
            IBigGamingServices services,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations
        )
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _methodMapper = new Dictionary<string, Func<BigGamingRequest, Task<BigGamingResponse>>>
            {
                {"open.operator.user.balance", GetBalanceAsync},
                {"open.operator.order.transfer", BetAsync},
                {"open.operator.calc.transfer", SettleAsync},
                {"open.operator.order.status", OrderStatusAsync},
                {"open.operator.ping", PingAsync}
            };
            _jsonSerializerOptions = new()
            {
                IgnoreReadOnlyProperties = true,
                WriteIndented = true,
                //略過所有預設值屬性
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
        }

        [HttpPost("entrance")]
        public async Task<BigGamingResponse> EntranceAsync(
            BigGamingRequest baseRequest)
        {
            if (!_methodMapper.TryGetValue(baseRequest.Method.ToLower(), out var execute))
            {
                return new()
                {
                    Id = baseRequest.Id,
                    Error = new()
                    {
                        Code = GameApiError.TargetNotFound.ToString(),
                        Message = GameApiError.TargetNotFound.Description(),
                        Action = baseRequest.Method.ToLower()
                    },
                    Jsonrpc = baseRequest.Jsonrpc
                };
            }

            return await execute(baseRequest).ConfigureAwait(false);
        }

        [HttpPost("balance")]
        public async Task<BigGamingResponse> GetBalanceAsync(
            BigGamingRequest baseRequest)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return new()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(getBalance.Balance)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("bet")]
        public async Task<BigGamingResponse> BetAsync(
            BigGamingRequest baseRequest)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<BigGamingBetRequest>(
                _jsonSerializationProvider.SerializeToString(baseRequest.Params), _jsonSerializerOptions);

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.BigGaming);

            var betData = _map.Request(gameProviderConfigurationData, data, configuration.OppositeBets);
            var betResult = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            var result = new BigGamingBetResponse
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = betResult.Balance,
                OrderResult = "1",
                TranId = data.TranId
            };

            return new()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("settle")]
        public async Task<BigGamingResponse> SettleAsync(
            BigGamingRequest baseRequest)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<BigGamingSettleRequest>(
                _jsonSerializationProvider.SerializeToString(baseRequest.Params), _jsonSerializerOptions);
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.BigGaming);

            var balance = default(decimal);
            if (data.Recalc == "1")
            {
                var resettleResponse = await _map
                    .ResettleAsync(gameProviderConfigurationData, data, playerCode, configuration.OppositeBets)
                    .ConfigureAwait(false);

                if (resettleResponse.BeforeCancel.Transactions.Length > 0)
                {
                    var betDataResult = await _actions.CancelTransactionAsync(resettleResponse.BeforeCancel).ConfigureAwait(false);
                    balance = betDataResult.Balance;
                }

                if (resettleResponse.SettleBetData.Transactions.Length > 0)
                {
                    var betDataResult = await _actions.SettleBetAsync(resettleResponse.SettleBetData).ConfigureAwait(false);
                    balance = betDataResult.Balance;
                }

                if (resettleResponse.CancelBetData.Transactions.Length > 0)
                {
                    var betDataResult = await _actions.CancelTransactionAsync(resettleResponse.CancelBetData).ConfigureAwait(false);
                    balance = betDataResult.Balance;
                }
            }
            else
            {
                var settleResponse = await _map
                    .SettleAsync(gameProviderConfigurationData, data, playerCode, configuration.OppositeBets)
                    .ConfigureAwait(false);

                if (settleResponse.SettleBetData.Transactions.Length > 0)
                {
                    var betDataResult = await _actions.SettleBetAsync(settleResponse.SettleBetData).ConfigureAwait(false);
                    balance = betDataResult.Balance;
                }

                if (settleResponse.CancelBetData.Transactions.Length > 0)
                {
                    var betDataResult = await _actions.CancelTransactionAsync(settleResponse.CancelBetData).ConfigureAwait(false);
                    balance = betDataResult.Balance;
                }
            }

            var result = new BigGamingSettleResponse
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = balance,
                OrderResult = "1",
                TranId = data.TranId
            };

            return new()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("orderstatus")]
        public async Task<BigGamingResponse> OrderStatusAsync(
            BigGamingRequest baseRequest)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<BigGamingOrderStatusRequest>(
                _jsonSerializationProvider.SerializeToString(baseRequest.Params), _jsonSerializerOptions);
            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var orderStatusResultResponse = new List<BigGamingOrderStatusResultResponse>();

            foreach (var order in data.OrderMap)
            {
                orderStatusResultResponse.Add(new()
                {
                    OrderId = order.OrderId,
                    Status = (int)await _services
                        .GetOrderStatusAsync(gameProviderId, order.LoginId, order.OrderId.ToString())
                        .ConfigureAwait(false)
                });
            }


            var result = new BigGamingOrderStatusResponse { Result = orderStatusResultResponse.ToArray() };

            return new()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
        }

        [HttpPost("ping")]
        public async Task<BigGamingResponse> PingAsync(
            BigGamingRequest baseRequest) =>
            new()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(
                    JsonSerializer.Serialize($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}")),
                Error = null,
                Jsonrpc = baseRequest.Jsonrpc
            };
    }
}
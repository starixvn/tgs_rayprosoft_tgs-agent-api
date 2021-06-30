using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SexyGaming.Common;
using TGS.Shared.Game.Models.SexyGaming.Incoming;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;
using TGS.Shared.Game.Models.SexyGaming.Enums;
using TGS.Apis.TgsApi.GameProviders.SexyGaming.Services;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Controllers
{
    [Route("api/sg")]
    [ApiController]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.SexyGaming)]
    public sealed class SexyGamingController : ControllerBase
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGamePlayActions _actions;
        private readonly ISettingOperations _settingOperations;
        private readonly ISexyGamingActionModelMapper _map;
        private readonly IReadOnlyDictionary<string, Func<JsonDocument, Task<JsonDocument>>> _methodMapper;
        private readonly ISexyGamingServices _services;
        public SexyGamingController(
            IGamePlayActions actions,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations,
            ISexyGamingActionModelMapper map,
            ISexyGamingServices services
        )
        {

            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _methodMapper = new Dictionary<string, Func<JsonDocument, Task<JsonDocument>>>
            {
                {"getBalance", GetBalanceAsync},
                {"bet", BetAsync},
                {"cancelBet", CancelBetAsync},
                {"give", GiveAsync},
                {"settle", SettleAsync},
                {"unsettle", UnsettleAsync},
                {"voidBet", VoidBetAsync},
                {"voidSettle", VoidSettleAsync},
                {"ping", PingAsync},
            };

        }

        [HttpPost("entrance")]
        public async Task<JsonDocument> EntranceAsync(
           [FromForm] SexyGamingSingleWalletRequest baseRequest)
        {
            var jsonMessage = JsonDocument.Parse(baseRequest.Message);
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletBaseRequest>(
                _jsonSerializationProvider.SerializeToString(jsonMessage));
            if (!_methodMapper.TryGetValue(data.Action, out var execute))
            {
                throw new InvalidOperationException();
            }

            return await execute(jsonMessage).ConfigureAwait(false);
        }

        [HttpPost("balance")]
        public async Task<JsonDocument> GetBalanceAsync(JsonDocument request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
                Balance = getBalance.Balance.Truncate(3),
                BalanceTs = DateTimeOffset.UtcNow.ToIso8601String(true)
            }));
        }

        [HttpPost("bet")]
        public async Task<JsonDocument> BetAsync(JsonDocument request)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletTxnsRequest<SexyGamingBet>>(
                _jsonSerializationProvider.SerializeToString(request));

            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.BetAsync(gameProviderConfigurationData, playerCode, data.Transaction).ConfigureAwait(false);

            if (betData != null)
            {
                var betResult = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
                return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
                {
                    Status = ((int)GameApiError.Success).ToString("d4"),
                    Desc = GameApiError.Success.Description(),
                    Balance = betResult.Balance.Truncate(3),
                    BalanceTs = DateTimeOffset.UtcNow.ToIso8601String(true)
                }));
            }
            else
            {
                var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.SexyGaming, playerCode).ConfigureAwait(false);
                return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
                {
                    Status = ((int)GameApiError.BetHasCanceled).ToString("d4"),
                    Desc = GameApiError.BetHasCanceled.Description(),
                    Balance = getBalance.Balance.Truncate(3),
                    BalanceTs = DateTimeOffset.UtcNow.ToIso8601String(true)
                }));
            }


        }

        [HttpPost("cancelBet")]
        public async Task<JsonDocument> CancelBetAsync(JsonDocument request)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletTxnsRequest<SexyGamingCancelBet>>(
                _jsonSerializationProvider.SerializeToString(request));

            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.CancelBetAsync(gameProviderConfigurationData, playerCode, data.Transaction).ConfigureAwait(false);
            decimal balance;
            if (betData.Transactions.Length > 0)
            {
                var betResult = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
                balance = betResult.Balance;
            }
            else
            {
                var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.SexyGaming, playerCode).ConfigureAwait(false);
                balance = getBalance.Balance;
            }

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
                Balance = balance.Truncate(3),
                BalanceTs = DateTimeOffset.UtcNow.ToIso8601String(true)
            }));
        }

        [HttpPost("voidBet")]
        public async Task<JsonDocument> VoidBetAsync(JsonDocument request)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletTxnsRequest<SexyGamingVoidBett>>(
                _jsonSerializationProvider.SerializeToString(request));

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.VoidBetAsync(gameProviderConfigurationData, data.Transaction).ConfigureAwait(false);
            await _actions.CancelBatchAsync(betData).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("settle")]
        public async Task<JsonDocument> SettleAsync(JsonDocument request)
        {

            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletTxnsRequest<SexyGamingSettle>>(
                _jsonSerializationProvider.SerializeToString(request));


            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SexyGamingConfiguration>(
                    SettingConsts.GameProviderConfigurations.SexyGaming);

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.SettleAsync(gameProviderConfigurationData, data.Transaction, configuration.OppositeBets).ConfigureAwait(false);
            await _actions.SettleBatchAsync(betData).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("unsettle")]
        public async Task<JsonDocument> UnsettleAsync(JsonDocument request)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletTxnsRequest<SexyGamingUnsettle>>(
                _jsonSerializationProvider.SerializeToString(request));

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.UnsettleAsync(gameProviderConfigurationData, data.Transaction).ConfigureAwait(false);
            await _actions.CancelBatchAsync(betData).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("voidSettle")]
        public async Task<JsonDocument> VoidSettleAsync(JsonDocument request)
        {
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletTxnsRequest<SexyGamingVoidSettle>>(
                _jsonSerializationProvider.SerializeToString(request));

            var items = HttpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.VoidSettleAsync(gameProviderConfigurationData, playerCode, data.Transaction).ConfigureAwait(false);
            await _actions.CancelBatchAsync(betData).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("give")]
        public Task<JsonDocument> GiveAsync(JsonDocument request)
            => Task.FromResult(
                JsonDocument.Parse(
                    _jsonSerializationProvider.SerializeToString(
                        new SexyGamingGetBalanceResponse()
                        {
                            Status = ((int)GameApiError.Fail).ToString("d4"),
                            Desc = GameApiError.Fail.Description(),
                            Balance = default(decimal),
                            BalanceTs = string.Empty
                        })));

        [HttpPost("ping")]
        public Task<JsonDocument> PingAsync(JsonDocument request)
        {
            return Task.FromResult(JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            })));
        }
    }
}


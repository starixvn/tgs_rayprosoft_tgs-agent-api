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
using TGS.Shared.Game.Models.SexyGaming.Common;
using TGS.Shared.Game.Models.SexyGaming.Incoming;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;
using TGS.Shared.Game.Models.SexyGaming.Enums;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Controllers
{
    [Route("api/sg")]
    [ApiController]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.SexyGaming)]
    public sealed class BigGamingController : ControllerBase
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGamePlayActions _actions;
        private readonly ISettingOperations _settingOperations;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IReadOnlyDictionary<string, Func<SexyGamingSingleWalletRequest, Task<JsonDocument>>> _methodMapper;

        public BigGamingController(
            IGamePlayActions actions,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations
        )
        {

            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _methodMapper = new Dictionary<string, Func<SexyGamingSingleWalletRequest, Task<JsonDocument>>>
            {
                {"getBalance", GetBalanceAsync},
                {"bet", BetAsync},
                {"cancelBet", CancelBetAsync},
                {"give", BetAsync},
                {"settle", SettleAsync},
                {"unsettle", UnsettleAsync},
                {"voidBet", VoidBetAsync},
                {"voidSettle", VoidSettleAsync},
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
        public async Task<JsonDocument> EntranceAsync(
           [FromForm] SexyGamingSingleWalletRequest baseRequest)
        {
            var messages = JsonSerializer.Deserialize<Dictionary<string, string>>(baseRequest.Message);
            var data = _jsonSerializationProvider.DeserializeFromString<SexyGamingSingleWalletBaseRequest>(
                _jsonSerializationProvider.SerializeToString(messages), _jsonSerializerOptions);
            if (!_methodMapper.TryGetValue(data.Action, out var execute))
            {
                throw new InvalidOperationException();
            }

            return await execute(baseRequest).ConfigureAwait(false);
        }

        [HttpPost("balance")]
        public async Task<JsonDocument> GetBalanceAsync(
            SexyGamingSingleWalletRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
                Balance = getBalance.Balance,
                BalanceTs = DateTimeOffset.UtcNow
            }));
        }

        [HttpPost("bet")]
        public async Task<JsonDocument> BetAsync(
            SexyGamingSingleWalletRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
                Balance = getBalance.Balance,
                BalanceTs = DateTimeOffset.UtcNow
            }));
        }

        [HttpPost("cancelBet")]
        public async Task<JsonDocument> CancelBetAsync(
            SexyGamingSingleWalletRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
                Balance = getBalance.Balance,
                BalanceTs = DateTimeOffset.UtcNow
            }));
        }

        [HttpPost("voidBet")]
        public async Task<JsonDocument> VoidBetAsync(
            SexyGamingSingleWalletRequest request)
        {
            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("settle")]
        public async Task<JsonDocument> SettleAsync(
           SexyGamingSingleWalletRequest request)
        {
            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("unsettle")]
        public async Task<JsonDocument> UnsettleAsync(
          SexyGamingSingleWalletRequest request)
        {
            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("voidSettle")]
        public async Task<JsonDocument> VoidSettleAsync(
          SexyGamingSingleWalletRequest request)
        {
            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingBaseResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
            }));
        }

        [HttpPost("give")]
        public async Task<JsonDocument> GiveAsync(
          SexyGamingSingleWalletRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return JsonDocument.Parse(_jsonSerializationProvider.SerializeToString(new SexyGamingGetBalanceResponse()
            {
                Status = ((int)GameApiError.Success).ToString("d4"),
                Desc = GameApiError.Success.Description(),
                Balance = getBalance.Balance,
                BalanceTs = DateTimeOffset.UtcNow
            }));
        }


    }
}


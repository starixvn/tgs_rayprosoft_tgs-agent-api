using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Skywind.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Shared.Game.Models.Skywind.Common;
using TGS.Shared.Game.Models.Skywind.Enums;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Controllers
{
    [Route("api/sw")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Skywind)]
    public sealed class SkywindController : ControllerBase
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGamePlayActions _actions;
        private readonly ISettingOperations _settingOperations;
        private readonly ISkywindActionModelMapper _map;
        private readonly ISkywindServices _services;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGamePlayActions _gamePlayActions;
        public SkywindController(
            IGamePlayActions actions,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations,
            ISkywindActionModelMapper map,
            ISkywindServices services,
            IGameProviderOperations gameProviderOperations,
            IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper,
            IPlayerOperations playerOperations,
            IGamePlayActions gamePlayActions)
        {

            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gamePlayActions = gamePlayActions ?? throw new ArgumentNullException(nameof(gamePlayActions));
        }

        [HttpPost("get_balance")]
        public async Task<SkywindSingleWalletResponse> GetBalanceAsync(SkywindBalanceRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = getBalance.Balance
            };
        }

        [HttpPost("debit")]
        public async Task<SkywindSingleWalletResponse> BetAsync(SkywindBetRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = _map.BetAsync(gameProviderConfigurationData, playerCode, new[] { request });
            var betResult = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = betResult.Balance,
                TrxId = request.TrxId
            };
        }

        [HttpPost("credit")]
        public async Task<SkywindSingleWalletResponse> SettleAsync(SkywindSettleRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = _map.SettleAsync(gameProviderConfigurationData, playerCode, new[] { request });
            var betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = betResult.Balance,
                TrxId = request.TrxId
            };
        }


        [HttpPost("rollback")]
        public async Task<SkywindSingleWalletResponse> RollbackBetAsync(SkywindRollbackBetRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.RollbackBetAsync(gameProviderConfigurationData, playerCode, new[] { request }).ConfigureAwait(false);
            var balance = default(decimal);
            if (betData.Transactions.Length > 0)
            {
                var betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
                balance = betResult.Balance;
            }
            else
            {
                var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);
                balance = getBalance.Balance;
            }

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = balance
            };
        }

        [HttpPost("validate_ticket")]
        public async Task<SkywindValidateTicketResponse> ValidateTicketAsync(SkywindValidateTicketRequest request)
        {

            var loginRecord = await _services.DecryptTokenAsync(request.Ticket);

            var validationResult = await _gamePlayActions.ValidatePlayerAsync(new PlayerValidationData()
            {
                PlayerCode = loginRecord.PlayerCode,
                GameProviderCode = Consts.GameProvider.Skywind,
                PlatformType = loginRecord.PlatformType
            }).ConfigureAwait(false);

            return new SkywindValidateTicketResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                CustId = loginRecord.PlayerCode,
                CustSessionId = request.Ticket,
                CurrencyCode = validationResult.GameProviderCurrencyCode,
                Country = "CN",
                TestCust = false
            };
        }

        [HttpPost("ping")]
        public SkywindSingleWalletResponse Ping() => new SkywindSingleWalletResponse()
        {
            ErrorCode = (int)GameApiError.Success,
            ErrorMsg = GameApiError.Success.Description(),
        };

    }
}

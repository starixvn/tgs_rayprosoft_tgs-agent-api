using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.BetConstruct.Enums;
using TGS.Shared.Game.Models.BetConstruct.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Controllers
{
    [Route("bc/api")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.BetConstruct)]
    public sealed class BetConstructController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.BetConstruct;

        private readonly IGamePlayActions _actions;
        private readonly IBetConstructActionModelMapper _map;
        private readonly IBetConstructService _service;

        public BetConstructController(
            IGamePlayActions actions,
            IBetConstructService service,
            IBetConstructActionModelMapper map
            )
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        [HttpPost("BetConfirm")]
        public ValueTask BetConfirm(BetConfirmRequest request)
        {
            return ValueTask.CompletedTask;
        }

        [HttpPost("BetPlaced")]
        public async ValueTask<BetPlacedResponse> BetPlacedAsync(BetPlacedRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _service
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = await _map.BetAsync(gameProviderConfigurationData, playerCode, request);

            if (betData == null)
            {
                return new BetPlacedResponse()
                {
                    ErrorCode = $"{(int)GameApiError.InternalError}",
                    ErrorText = GameApiError.InternalError.Description()
                };
            }

            return new BetPlacedResponse()
            {
                ErrorCode = $"{(int)GameApiError.Success}",
                ErrorText = GameApiError.Success.Description()
            };
        }

        [HttpPost("BetResulted")]
        public async ValueTask<BetResultedResponse> BetResulted(BetResultedRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _service
               .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
               .ConfigureAwait(false);

            var betData = await _map.SettleAsync(gameProviderConfigurationData, playerCode, request).ConfigureAwait(false);

            if (betData == null)
            {
                return new BetResultedResponse()
                {
                    ErrorCode = $"{(int)GameApiError.InternalError}",
                    ErrorText = GameApiError.InternalError.Description()
                };
            }

            var betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);

            return new BetResultedResponse()
            {
                ErrorCode = $"{(int)GameApiError.Success}",
                ErrorText = GameApiError.Success.Description()
            };
        }

        [HttpPost("GetClientBalance")]
        public async ValueTask<GetClientBalanceResponse> GetClientBalanceAsync(GetClientBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _actions
                .GetBalanceAsync(GameProviderCode, playerCode)
                .ConfigureAwait(false);

            return new GetClientBalanceResponse()
            {
                Balance = r.Balance,
                ErrorCode = $"{(int)GameApiError.Success}",
                ErrorText = GameApiError.Success.Description()
            };
        }

        [HttpPost("GetClientDetails")]
        public async ValueTask<GetClientDetailsResponse> GetClientDetails(GetClientDetailsRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = HttpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var player = await _service
                .GetPlayerByCodeAsync(playerCode)
                .ConfigureAwait(false);
            var currencyId = await _service
                .GetGameProviderCurrenciesByInternalCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
                .ConfigureAwait(false);

            return new GetClientDetailsResponse()
            {
                Login = player.PlayerCode,
                CurrencyId = currencyId,
                ExternalId = player.PlayerCode,
                ErrorCode = ((int)GameApiError.Success).ToString(),
                ErrorText = GameApiError.Success.Description()
            };
        }

        [HttpPost("Rollback")]
        public async ValueTask<RollbackResponse> Rollback(RollbackRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _service
               .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
               .ConfigureAwait(false);

            var betData = await _map
                .RollbackBetAsync(gameProviderConfigurationData, playerCode, request)
                .ConfigureAwait(false);

            if (betData == null)
            {
                return new RollbackResponse()
                {
                    ErrorCode = $"{(int)GameApiError.InternalError}",
                    ErrorText = GameApiError.InternalError.Description()
                };
            }

            _ = await _actions.SettleBetAsync(betData).ConfigureAwait(false);

            return new RollbackResponse()
            {
                ErrorCode = $"{(int)GameApiError.Success}",
                ErrorText = GameApiError.Success.Description()
            };
        }
    }
}

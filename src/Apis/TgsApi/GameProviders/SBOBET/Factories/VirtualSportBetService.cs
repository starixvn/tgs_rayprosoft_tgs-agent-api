using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    internal sealed class VirtualSportBetService : BaseBetService
    {
        private readonly IGamePlayActions _actions;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly ISBOBETActionModelMapper _map;
        private readonly ISBOBETServices _services;
        private readonly ISettingOperations _settingOperations;

        public VirtualSportBetService(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations)
            : base(httpContextAccessor, actions, map,
                services, settingOperations)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _actions = actions;
            _map = map;
            _services = services;
            _gameProviderOperations = gameProviderOperations;
            _settingOperations = settingOperations;
        }

        public override async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var configuration =
                await _settingOperations
                    .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET)
                    .ConfigureAwait(false);

            if (configuration.DelaySecondsForPlaceBet > 0)
            {
                await Task.Delay(configuration.DelaySecondsForPlaceBet * 1000).ConfigureAwait(false);
            }

            var items = _httpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var (platformType, _) = await _services
                .GetPlatformTypeAndGameWithLoginRecordsAsync(gameProviderId, request.ExternalGameId,
                    request.PlayerCode).ConfigureAwait(false);

            var data = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var betData = _map.Request(data, request, true);
            var r = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r);
        }

        public override async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var items = _httpContext.Items;
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var externalRoundId = request.ExternalTransactionId;
            var playerCode = request.PlayerCode;

            var (roundStatus, gameId, platformType) =
                await _services.GetRoundStatusAndGameIdAndPlatformAsync(gameProviderId, playerCode,
                    externalRoundId).ConfigureAwait(false);

            var data = await _services
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            var gameActionExternalIds =
                await _services.GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId);

            if (roundStatus == RoundStatus.Closed && gameActionExternalIds.Length == 0)
            {
                return _map.Response<SettleBetResponse>(playerCode, GameApiError.BetAlreadyCanceled);
            }

            var betData = _map.Request(data, externalRoundId, request, roundStatus, gameActionExternalIds);
            var r = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r);
        }
    }
}
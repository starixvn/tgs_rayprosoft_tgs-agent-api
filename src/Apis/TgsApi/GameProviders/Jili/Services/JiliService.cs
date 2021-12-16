using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public class JiliService : IJiliService
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly ILogger<JiliService> _logger;
        private readonly ISettingOperations _settingOperations;

        public JiliService(
            ILogger<JiliService> logger,
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _httpContext = httpContextAccessor?.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async ValueTask<JiliConfiguration> GetConfigurationAsync()
            => await _settingOperations
                .GetValueFromJsonByKeyAsync<JiliConfiguration>(SettingConsts.GameProviderConfigurations.Jili)
                .ConfigureAwait(false);

        public async ValueTask<GameActionData> GetGameActionsByExternalRoundIdAsync(
            Guid gameProviderId,
            string playerCode,
            string externalRoundId,
            string externalGameId)
        {
            var items = _httpContext?.Items ??
                        throw new InvalidOperationException("Can not get http context items");

            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var games = await _gameProviderOperations
                .GetCachedGamesAsync(gameProviderId, externalGameId)
                .ConfigureAwait(false);

            var gamePlatformTypes = platformType.ToGamePlatformTypes();
            var gameId = games.Where(g => gamePlatformTypes.Contains(g.PlatformType))
                .Select(g => g.Id).FirstOrDefault();

            return await _gameActionQueries
                .GetGameActionsByExternalRoundIdAsync(
                    gameProviderId,
                    playerCode,
                    externalRoundId,
                    gameId)
                .ConfigureAwait(false);
        }

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContext?.Items ??
                           throw new InvalidOperationException("Can not get http context items");

            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }

        public async ValueTask<Guid> GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Jili)
                .ConfigureAwait(false);
    }
}

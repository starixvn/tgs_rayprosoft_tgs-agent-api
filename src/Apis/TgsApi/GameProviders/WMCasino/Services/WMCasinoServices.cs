using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public class WMCasinoServices : IWMCasinoServices
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;

        public WMCasinoServices(
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
        }

        public ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsByExternalGameActionGroupIdAsync(
            Guid gameProviderId, string playerCode, string externalGameActionGroupId)
            => _gameActionQueries.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(gameProviderId,
                playerCode,
                externalGameActionGroupId);

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
                    Guid gameProviderId,
            Guid gameProviderConfigurationId, string playerCode, string externalGameId)
        {
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            var platformType = await GetPlatformTypeAsync(gameProviderId, externalGameId, playerCode)
                .ConfigureAwait(false);

            return new GameProviderConfigurationData
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataWithDesktopPlatformAsync(
            Guid gameProviderId, Guid gameProviderConfigurationId)
        {
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            return new GameProviderConfigurationData
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = PlatformType.Desktop
            };
        }

        private async ValueTask<PlatformType> GetPlatformTypeAsync(
            Guid gameProviderId, string externalGameId, string playerCode)
        {
            var platformTypes = new[]
            {
                PlatformType.Desktop,
                PlatformType.Mobile
            };

            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId)
                .ConfigureAwait(false);

            var gamePlatforms = games.Length == 0
                ? platformTypes
                : games.SelectMany(GetPlatformTypes).Distinct().ToArray();

            return gamePlatforms.DefaultIfEmpty(PlatformType.Desktop).FirstOrDefault();

            PlatformType[] GetPlatformTypes(CachedGame game) =>
                game.PlatformType switch
                {
                    GamePlatformType.DesktopAndMobile => platformTypes,
                    GamePlatformType.Desktop when game.Type == GameType.VirtualSports => platformTypes,
                    GamePlatformType.Desktop => new[]
                    {
                        PlatformType.Desktop
                    },
                    GamePlatformType.Mobile => new[]
                    {
                        PlatformType.Mobile
                    },
                    _ => throw new InvalidOperationException("Invalid game platform type mapping")
                };
        }
    }
}

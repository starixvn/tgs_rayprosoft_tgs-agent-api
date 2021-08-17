using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoServices
    {
        ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode);

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(string playerCode,
            string externalGameId);
    }

    public class WMCasinoServices : IWMCasinoServices
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.WMCasino;
        private readonly IGamePlayActions _actions;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly IPlayerOperations _playerOperations;

        public WMCasinoServices(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions actions,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations
        )
        {
            _httpContext = httpContextAccessor.HttpContext;
            _actions = actions;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
        }

        public ValueTask<GetBalanceResult> GetBalanceAsync(string playerCode)
            => _actions.GetBalanceAsync(_GameProviderCode, playerCode);

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(string playerCode,
            string externalGameId)
        {
            var gameProviderId =
                _httpContext.Items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId =
                _httpContext.Items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

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

            var loginRecords = await _playerOperations.GetPlayerEnabledLoginRecordsAsync(playerCode)
                .ConfigureAwait(false);

            var platformType = (from rec in loginRecords
                                join p in gamePlatforms on rec.PlatformType equals p
                                orderby rec.ExpiredOn descending
                                select (PlatformType?)rec.PlatformType).FirstOrDefault();

            if (!platformType.HasValue)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }

            return platformType.Value;

            PlatformType[] GetPlatformTypes(CachedGame game) =>
                game.PlatformType switch
                {
                    GamePlatformType.DesktopAndMobile                                 => platformTypes,
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
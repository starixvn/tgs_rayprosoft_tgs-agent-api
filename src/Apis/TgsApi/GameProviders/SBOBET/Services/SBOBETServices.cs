// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public interface ISBOBETServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, PlatformType platformType);

        ValueTask<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId);

        ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes);

        ValueTask<GameActionData> GetValidGameActionsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId,
            int gameId);

        ValueTask<(RoundStatus RoundStatus, int GameId, PlatformType PlatformType)>
            GetRoundStatusAndGameIdAndPlatformAsync(Guid gameProviderId, string playerCode, string externalRoundId);

        ValueTask<PlatformType> GetPlatformTypeWithGameAndLoginRecordsAsync(
            Guid gameProviderId, string externalGameId, string playerCode);
    }

    public sealed class SBOBETServices : ISBOBETServices
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly IPlayerOperations _playerOperations;
        private readonly IRoundQueries _roundQueries;

        public SBOBETServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IRoundQueries roundQueries,
            IPlayerOperations playerOperations)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _roundQueries = roundQueries;
            _playerOperations = playerOperations;
        }

        /// <summary>
        ///     Get game provider configuration data from http context
        /// </summary>
        async ValueTask<GameProviderConfigurationData> ISBOBETServices.GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContext?.Items;
            if (items == null)
            {
                throw new InvalidOperationException("Can not get http context items");
            }

            var gameProviderId = items.Get(ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(ContextItems.PlatformType, PlatformType.Desktop);

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

        async ValueTask<GameProviderConfigurationData> ISBOBETServices.GetGameProviderConfigurationDataAsync(
            Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
        {
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

        async ValueTask<GameActionExternalIds[]> ISBOBETServices.GetValidGameActionExternalIdsAsync(
            Guid gameProviderId, string playerCode, string externalRoundId, int gameId) =>
            await _gameActionQueries.GetValidGameActionExternalIdsAsync(gameProviderId,
                playerCode, externalRoundId, gameId, false).ConfigureAwait(false);

        async ValueTask<GameActionExternalIds[]> ISBOBETServices.GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes) =>
            await _gameActionQueries.GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId,
                gameActionTypes).ConfigureAwait(false);

        async ValueTask<GameActionData> ISBOBETServices.GetValidGameActionsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId) =>
            await _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId,
                gameId).ConfigureAwait(false);

        async ValueTask<(RoundStatus RoundStatus, int GameId, PlatformType PlatformType)> ISBOBETServices.
            GetRoundStatusAndGameIdAndPlatformAsync(Guid gameProviderId, string playerCode, string externalRoundId)
        {
            var externalTransactionId = externalRoundId;
            var data = await _roundQueries
                .GetRoundStatusAndGameIdAsync(gameProviderId, playerCode, externalRoundId,
                    externalTransactionId)
                .ConfigureAwait(false);

            if (!data.HasValue)
            {
                throw new InvalidGameActionException(
                    $"Data with referenced round is not found. RoundId: {externalRoundId}",
                    InvalidGameActionErrorCode.RoundNotFound);
            }

            return (data.Value.RoundStatus, data.Value.GameId, PlatformType.Desktop);
        }

        async ValueTask<PlatformType> ISBOBETServices.
            GetPlatformTypeWithGameAndLoginRecordsAsync(Guid gameProviderId, string externalGameId,
                string playerCode)
        {
            var platformTypes = new[] {PlatformType.Desktop, PlatformType.Mobile};
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId);

            var gamePlatforms = games.Length == 0
                ? platformTypes
                : games.SelectMany(GetPlatformTypes).Distinct().ToArray();

            var loginRecords = await _playerOperations.GetPlayerEnabledLoginRecordsAsync(playerCode);
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
                    GamePlatformType.DesktopAndMobile => platformTypes,
                    GamePlatformType.Desktop          => new[] {PlatformType.Desktop},
                    GamePlatformType.Mobile           => new[] {PlatformType.Mobile}
                };
        }
    }
}
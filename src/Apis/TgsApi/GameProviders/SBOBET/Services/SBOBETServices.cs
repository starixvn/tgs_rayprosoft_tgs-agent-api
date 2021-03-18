// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
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

        ValueTask<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId);

        ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes);

        ValueTask<GameActionData> GetValidGameActionsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId,
            int gameId);

        ValueTask<(RoundStatus? RoundStatus, int GameId)> GetRoundStatusAndGameIdAsync(Guid gameProviderId,
            GameProviderConfigurationData data, string playerCode, string externalRoundId, string externalGameId,
            int gameProviderGameType);
    }

    public sealed class SBOBETServices : ISBOBETServices
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly IRoundQueries _roundQueries;

        public SBOBETServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IRoundQueries roundQueries)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _roundQueries = roundQueries;
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

        async ValueTask<(RoundStatus? RoundStatus, int GameId)> ISBOBETServices.GetRoundStatusAndGameIdAsync(
            Guid gameProviderId, GameProviderConfigurationData data, string playerCode,
            string externalRoundId, string externalGameId, int gameProviderGameType)
        {
            RoundStatus? roundStatus;
            int? gameId;

            if (gameProviderGameType > 0)
            {
                gameId = await GetGameIdAsync(gameProviderId, externalGameId, data.PlatformType)
                    .ConfigureAwait(false);

                roundStatus =
                    await _roundQueries.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId.Value)
                        .ConfigureAwait(false);
            }
            else
            {
                var externalTransactionId = externalRoundId;
                (roundStatus, gameId) = await _roundQueries
                    .GetRoundStatusWithGameIdAsync(gameProviderId, playerCode, externalRoundId, externalTransactionId)
                    .ConfigureAwait(false);
            }

            if (!gameId.HasValue)
            {
                throw new InvalidGameActionException(
                    $"Game with referenced round is not found. RoundId: {externalRoundId}",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            return (roundStatus, gameId.Value);
        }

        private async ValueTask<int> GetGameIdAsync(Guid gameProviderId, string externalGameId,
            PlatformType platformType)
        {
            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, externalGameId, platformType)
                .ConfigureAwait(false);

            if (game == null)
            {
                throw new InvalidGameActionException(
                    $"Cannot find game '{externalGameId}' for platform '{platformType}' of provider with id={gameProviderId}",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            return game.Id;
        }
    }
}
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Linq;
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
        Task<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        Task<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId);

        Task<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes);

        Task<GameActionData> GetValidGameActionsAsync(Guid gameProviderId, string playerCode, string externalRoundId,
            int gameId);

        Task<RoundStatus?> GetRoundStatusAsync(Guid gameProviderId, string playerCode, string externalRoundId,
            int gameId);

        Task<Guid> GetPlayerDefaultWalletIdAsync(string playerCode);

        Task<int> GetGameIdAsync(Guid gameProviderId, string externalGameId, PlatformType platformType);
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
            IPlayerOperations playerOperations,
            IGameActionQueries gameActionQueries,
            IRoundQueries roundQueries)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _gameActionQueries = gameActionQueries;
            _roundQueries = roundQueries;
        }

        async Task<GameProviderConfigurationData> ISBOBETServices.GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContext?.Items;
            if (items == null)
            {
                throw new InvalidOperationException("Can not get http context items");
            }

            var gameProviderId = items.Get(ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(ContextItems.PlatformType, PlatformType.Desktop);

            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = cachedGameProvider.AutoCloseRound,
                PlatformType = platformType
            };
        }

        Task<GameActionExternalIds[]> ISBOBETServices.GetValidGameActionExternalIdsAsync(
            Guid gameProviderId, string playerCode, string externalRoundId, int gameId) =>
            _gameActionQueries.GetValidGameActionExternalIdsAsync(gameProviderId,
                playerCode, externalRoundId, gameId, false);

        Task<GameActionExternalIds[]> ISBOBETServices.GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes) =>
            _gameActionQueries.GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId,
                gameActionTypes);

        Task<RoundStatus?> ISBOBETServices.GetRoundStatusAsync(Guid gameProviderId, string playerCode,
            string externalRoundId,
            int gameId) =>
            _roundQueries.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId);

        Task<Guid> ISBOBETServices.GetPlayerDefaultWalletIdAsync(string playerCode) =>
            _playerOperations.GetDefaultWalletIdByPlayerCodeAsync(playerCode);

        async Task<int> ISBOBETServices.GetGameIdAsync(Guid gameProviderId, string externalGameId,
            PlatformType platformType)
        {
            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, externalGameId, platformType);
            
            if (game == null)
            {
                throw new InvalidGameActionException(
                    $"Cannot find game '{externalGameId}' for platform '{platformType}' of provider with id={gameProviderId}",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            return game.Id;
        }

        Task<GameActionData> ISBOBETServices.GetValidGameActionsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId) =>
            _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId,
                gameId);
    }
}
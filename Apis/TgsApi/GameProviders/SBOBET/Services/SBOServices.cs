using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public interface ISBOServices
    {
        Task<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        Task<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId);

        Task<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, GameActionType[] gameActionTypes);
    }

    public sealed class SBOServices : ISBOServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionQueries _gameActionQueries;

        public SBOServices(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _httpContextAccessor = httpContextAccessor;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
        }

        async Task<GameProviderConfigurationData> ISBOServices.GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContextAccessor.HttpContext.Items;

            var gameProviderId = (Guid)items[ContextItems.GameProviderId];
            var gameProviderConfigurationId = (Guid)items[ContextItems.GameProviderConfigurationId];
            var platformType = (PlatformType)items[ContextItems.PlatformType];

            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = cachedGameProvider.AutoCloseRound,
                PlatformType = platformType
            };
        }

        async Task<GameActionExternalIds[]> ISBOServices.GetValidGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId)
        {
            var walletId = await _playerOperations.GetMainWalletIdByPlayerIdAsync(playerCode);
            return await _gameActionQueries.GetValidGameActionExternalIdsAsync(gameProviderId, walletId, externalRoundId, false);
        }

        async Task<GameActionExternalIds[]> ISBOServices.GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, GameActionType[] gameActionTypes)
        {
            var walletId = await _playerOperations.GetMainWalletIdByPlayerIdAsync(playerCode);
            return await _gameActionQueries.GetGameActionExternalIdsAsync(gameProviderId, walletId, externalRoundId, gameActionTypes);
        }
    }
}
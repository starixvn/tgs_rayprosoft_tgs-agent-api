using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
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

        Task<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId);

        Task<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, GameActionType[] gameActionTypes);
        
        Task<GameActionData> GetValidGameActionsAsync(Guid gameProviderId, string playerCode, string externalRoundId);
    }

    public sealed class SBOBETServices : ISBOBETServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionQueries _gameActionQueries;

        public SBOBETServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _httpContextAccessor = httpContextAccessor;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
        }

        async Task<GameProviderConfigurationData> ISBOBETServices.GetGameProviderConfigurationDataAsync()
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

        Task<GameActionExternalIds[]> ISBOBETServices.GetValidGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId)
        {
            return _gameActionQueries.GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, false);
        }

        Task<GameActionExternalIds[]> ISBOBETServices.GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, GameActionType[] gameActionTypes)
        {
            return _gameActionQueries.GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameActionTypes);
        }

        Task<GameActionData> ISBOBETServices.GetValidGameActionsAsync(Guid gameProviderId, string playerCode, string externalRoundId)
        {
            return _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId);
        }
    }
}
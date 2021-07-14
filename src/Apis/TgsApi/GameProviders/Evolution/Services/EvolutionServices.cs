using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public interface IEvolutionServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes);
        ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsByExternalGameActionGroupIdAsync(Guid gameProviderId, string playerCode, string externalGameActionGroupId);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<CachedPlayerLoginRecord> DecryptTokenAsync(string token);
        ValueTask<Guid> GetGameProviderIdAsync();
    }

    public class EvolutionServices : IEvolutionServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameActionQueries _gameActionQueries;

        public EvolutionServices(
            IGameProviderOperations gameProviderOperations,
             ITokenProvider tokenProvider,
             IPlayerOperations playerOperations,
             IGameActionQueries gameActionQueries)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
        }

        async ValueTask<GameActionExternalIds> IEvolutionServices.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<GameActionExternalIds[]> IEvolutionServices.GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes)
           => await _gameActionQueries.GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId, gameActionTypes).ConfigureAwait(false);

        public async ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsByExternalGameActionGroupIdAsync(Guid gameProviderId, string playerCode, string externalGameActionGroupId)
            => await _gameActionQueries.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(gameProviderId, playerCode, externalGameActionGroupId).ConfigureAwait(false);

        async ValueTask<GameProviderConfigurationData> IEvolutionServices.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
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

        async ValueTask<CachedPlayerLoginRecord> IEvolutionServices.DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            return await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
        }

        async ValueTask<Guid> IEvolutionServices.GetGameProviderIdAsync()
        {
            return await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Evolution).ConfigureAwait(false);
        }


    }
}

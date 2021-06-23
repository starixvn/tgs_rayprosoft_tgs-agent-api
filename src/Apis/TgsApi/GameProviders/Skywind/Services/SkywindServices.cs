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

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<CachedPlayerLoginRecord> DecryptTokenAsync(string token);
        ValueTask<Guid> GetGameProviderIdAsync();
        string GetExternalRoundId(string roundId, long eventId);
        string GetExternalTransactionId(string trxId, long eventId, string action);
    }

    public class SkywindServices : ISkywindServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameActionQueries _gameActionQueries;

        public SkywindServices(
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

        async ValueTask<GameActionExternalIds> ISkywindServices.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<GameProviderConfigurationData> ISkywindServices.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
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

        async ValueTask<CachedPlayerLoginRecord> ISkywindServices.DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            return await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
        }

        async ValueTask<Guid> ISkywindServices.GetGameProviderIdAsync()
        {
            return await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Skywind).ConfigureAwait(false);
        }

        string ISkywindServices.GetExternalRoundId(string roundId, long eventId) => $"{roundId}_{eventId}";

        string ISkywindServices.GetExternalTransactionId(string trxId, long eventId, string action)
        {

            var externalTransaction = $"{trxId}_{eventId}";
            if (string.IsNullOrEmpty(action))
            {
                return externalTransaction;
            }
            else
            {
                return $"{externalTransaction}_{action}";
            }

        }
    }
}

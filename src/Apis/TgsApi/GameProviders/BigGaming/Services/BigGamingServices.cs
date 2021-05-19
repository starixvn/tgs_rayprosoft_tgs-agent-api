using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionData> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<decimal> GetPlayerBalanceAsync(string playerCode);
    }
    internal sealed class BigGamingServices : IBigGamingServices
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IRoundQueries _roundQueries;

        public BigGamingServices(
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IRoundQueries roundQueries,
            IPlayerOperations playerOperations)
        {
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _roundQueries = roundQueries;
            _playerOperations = playerOperations;
        }


        async ValueTask<GameActionExternalIds> IBigGamingServices.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<GameActionData> IBigGamingServices.GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
            => await _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId, gameId).ConfigureAwait(false);


        async ValueTask<decimal> IBigGamingServices.GetPlayerBalanceAsync(string playerCode)
        {
            var walletId = await _playerOperations.GetDefaultWalletIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);

            return await _playerOperations.GetPlayerBalanceAsync(walletId, false).ConfigureAwait(false);
        }

        async ValueTask<GameProviderConfigurationData> IBigGamingServices.GetGameProviderConfigurationDataAsync(
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
    }
}

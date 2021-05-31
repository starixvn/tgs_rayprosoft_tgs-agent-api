using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public interface ISexyGamingServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
    }
    internal sealed class SexyGamingServices : ISexyGamingServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionQueries _gameActionQueries;
        public SexyGamingServices(
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
        }

        async ValueTask<GameActionExternalIds> ISexyGamingServices.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<GameProviderConfigurationData> ISexyGamingServices.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
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

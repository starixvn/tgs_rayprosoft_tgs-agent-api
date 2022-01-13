using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.TF.Services
{
    public interface ITFService
    {        
        ValueTask<(RoundRecord, GameActionRecord)> GetGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(
            Guid gameProviderId,
            string playerCode,
            string externalTransactionId);

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
            Guid gameProviderId,
            Guid gameProviderConfigurationId,
            PlatformType platformType);

        ValueTask<string> GetGameProviderCurrenciesByInternalCodeAsync(
            Guid gameProviderId, string currencyCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<(RoundRecord, GameActionRecord)> GetSettleGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);
    }

    public class TFService : ITFService
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;

        public TFService(
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
        }        

        ValueTask<(RoundRecord, GameActionRecord)> ITFService.GetGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId) => throw new System.NotImplementedException();

        async ValueTask<GameActionExternalIds> ITFService.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<GameProviderConfigurationData> ITFService.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
        {
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
               .ConfigureAwait(false);

            return new GameProviderConfigurationData()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }

        ValueTask<string> ITFService.GetGameProviderCurrenciesByInternalCodeAsync(Guid gameProviderId, string currencyCode) => throw new System.NotImplementedException();

        async ValueTask<string> ITFService.GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
           => await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(currencyCode, gameProviderId).ConfigureAwait(false);

        async ValueTask<Guid> ITFService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.TFGaming).ConfigureAwait(false);

        async ValueTask<(RoundRecord, GameActionRecord)> ITFService.GetSettleGameActionByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
        {
            var data = await _gameActionQueries
                .GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId, gameId)
                .ConfigureAwait(false);

            var gameActionBetPlaced = data.GameActions
                .Where(x => x.Type == GameActionType.PlaceBet)
                .FirstOrDefault();

            var gameActionSettle = data.GameActions
                .Where(x => x.ReferencedGameActionId == gameActionBetPlaced.Id)
                .OrderByDescending(x => x.RecordedOn)
                .FirstOrDefault();

            return new(data.Round, gameActionSettle);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.TCG;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.GameProviders.Repositories.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public interface ITCGService
    {
        ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);

        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, int gameId);

        ValueTask<CachedPlayer> GetCachedPlayerAsync(string playerCode);

        ValueTask<TCGConfiguration> GetConfigurationAsync();

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

        ValueTask<(RoundRecord, GameActionRecord)> GetGameActionsByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);

        ValueTask<GameActionData> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

        ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);

        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);

        ValueTask<PlayerLoginRecordModel> GetPlayerLatestEnabledLoginRecordAsync(string playerCode);

        ValueTask<RoundStatus?> GetRoundStatusAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);

        ValueTask<bool> ReferencedGameActionExistsAsync(Guid gameProviderId, string playerCode, string externalRoundId, string externalTransactionId);
    }

    public class TCGService : ITCGService
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerLoginRecordService _playerLoginRecordService;
        private readonly IPlayerOperations _playerOperations;
        private readonly IRoundQueries _roundQueries;
        private readonly ISettingOperations _settingOperations;

        public TCGService(
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper,
            IBrandOperations brandOperations,
            IGameActionQueries gameActionQueries,
            IRoundQueries roundQueries,
            IPlayerLoginRecordService playerLoginRecordService)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _roundQueries = roundQueries ?? throw new ArgumentNullException(nameof(roundQueries));
            _playerLoginRecordService = playerLoginRecordService ?? throw new ArgumentNullException(nameof(playerLoginRecordService));
        }

        public async ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
            => await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

        public async ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode)
            => await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        public async ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, int gameId)
        => await _gameProviderOperations.GetCachedGameAsync(gameProviderId, gameId).ConfigureAwait(false);

        public async ValueTask<CachedPlayer> GetCachedPlayerAsync(string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                throw new PlayerLoginExpiredException();
            }
            return player;
        }

        public async ValueTask<TCGConfiguration> GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<TCGConfiguration>(SettingConsts.GameProviderConfigurations.TCG).ConfigureAwait(false);

        public ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId);

        public async ValueTask<(RoundRecord, GameActionRecord)> GetGameActionsByBetAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
        {
            var data = await _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId, gameId).ConfigureAwait(false);
            var gameAction = data.GameActions.Where(x => x.Type == GameActionType.PlaceBet).FirstOrDefault();
            return new(data.Round, gameAction);
        }

        public ValueTask<GameActionData> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
            => _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId, gameId);

        public string GetGameProviderCode()
        {
            return Consts.GameProvider.TCG;
        }

        public async ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language)
            => await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, language).ConfigureAwait(false);

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
            => await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(currencyCode, gameProviderId).ConfigureAwait(false);

        public async ValueTask<Guid> GetGameProviderIdAsync()
                                                                                                            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.TCG).ConfigureAwait(false);

        public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
        {
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<TCGConfiguration>(SettingConsts.GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        public async ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode)
        {
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<TCGConfiguration>(SettingConsts.GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.MerchantCode == merchantCode).FirstOrDefault();
        }

        public async ValueTask<Player> GetPlayerByCodeAsync(string playerCode)
                            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<PlayerLoginRecordModel> GetPlayerLatestEnabledLoginRecordAsync(string playerCode)
        => await _playerLoginRecordService.GetLastAsync(playerCode).ConfigureAwait(false);

        public ValueTask<RoundStatus?> GetRoundStatusAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
          => _roundQueries.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId);

        public ValueTask<bool> ReferencedGameActionExistsAsync(Guid gameProviderId, string playerCode, string externalRoundId, string externalTransactionId)
                    => _gameActionQueries.ReferencedGameActionExistsAsync(gameProviderId, playerCode, externalRoundId, externalTransactionId);
    }
}

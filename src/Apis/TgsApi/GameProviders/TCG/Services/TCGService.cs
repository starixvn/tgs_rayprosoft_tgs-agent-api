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
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public interface ITCGService
    {
        ValueTask<Guid> GetGameProviderIdAsync();
        ValueTask<TCGConfiguration> GetConfigurationAsync();
        ValueTask<CachedPlayer> GetCachedPlayerAsync(string playerCode);
        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);
        ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language);
        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);
        ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);
        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);
        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, int gameId);
        ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);
        ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);
        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);
        ValueTask<bool> ReferencedGameActionExistsAsync(Guid gameProviderId, string playerCode, string externalRoundId, string externalTransactionId);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<CachedPlayerLoginRecord> GetPlayerLatestEnabledLoginRecordAsync(string playerCode);
        ValueTask<RoundStatus?> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);
        ValueTask<GameActionData> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId);
        string GetGameProviderCode();
    }
    public class TCGService : ITCGService
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IRoundQueries _roundQueries;
        public TCGService(
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper,
            IBrandOperations brandOperations,
            IGameActionQueries gameActionQueries,
            IRoundQueries roundQueries)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _roundQueries = roundQueries ?? throw new ArgumentNullException(nameof(roundQueries));
        }
        async ValueTask<Guid> ITCGService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.TCG).ConfigureAwait(false);

        async ValueTask<TCGConfiguration> ITCGService.GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<TCGConfiguration>(SettingConsts.GameProviderConfigurations.TCG).ConfigureAwait(false);

        async ValueTask<CachedPlayer> ITCGService.GetCachedPlayerAsync(string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                throw new PlayerLoginExpiredException();
            }
            return player;
        }

        async ValueTask<Player> ITCGService.GetPlayerByCodeAsync(string playerCode)
            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);

        async ValueTask<string> ITCGService.GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language)
            => await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, language).ConfigureAwait(false);

        async ValueTask<string> ITCGService.GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
            => await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(currencyCode, gameProviderId).ConfigureAwait(false);

        async ValueTask<Guid> ITCGService.GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
            => await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

        async ValueTask<CachedGame> ITCGService.GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        string ITCGService.GetGameProviderCode()
        {
            return Consts.GameProvider.TCG;
        }

        async ValueTask<TCGMerchant> ITCGService.GetMerchantAsync(string currencyCode)
        {
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<TCGConfiguration>(SettingConsts.GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCodev == currencyCode).FirstOrDefault();
        }

        async ValueTask<CachedGame> ITCGService.GetCachedGameAsync(Guid gameProviderId, int gameId)
        => await _gameProviderOperations.GetCachedGameAsync(gameProviderId, gameId).ConfigureAwait(false);

        async ValueTask<TCGMerchant> ITCGService.GetMerchantByMerchantCodeAsync(string merchantCode)
        {
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<TCGConfiguration>(SettingConsts.GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.MerchantCode == merchantCode).FirstOrDefault();
        }

        async ValueTask<Guid> ITCGService.GetBrandIdByPlayerCodeAsync(string playerCode)
            => await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);

        async ValueTask<CachedPlayerLoginRecord> ITCGService.GetPlayerLatestEnabledLoginRecordAsync(string playerCode)
        => await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(playerCode).ConfigureAwait(false);

        ValueTask<bool> ITCGService.ReferencedGameActionExistsAsync(Guid gameProviderId, string playerCode, string externalRoundId, string externalTransactionId)
            => _gameActionQueries.ReferencedGameActionExistsAsync(gameProviderId, playerCode, externalRoundId, externalTransactionId);

        ValueTask<GameActionExternalIds> ITCGService.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId);

        ValueTask<RoundStatus?> ITCGService.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
          => _roundQueries.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId);

        ValueTask<GameActionData> ITCGService.GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
            => _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, externalRoundId, gameId);
    }
}

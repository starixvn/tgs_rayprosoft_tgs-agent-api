using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindService
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<CachedPlayerLoginRecord> DecryptTokenAsync(string token);
        ValueTask<Guid> GetGameProviderIdAsync();
        ValueTask<SkywindConfiguration> GetConfigurationAsync();
        ValueTask<Guid> GetGameProviderIdByCodeAsync();
        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);
        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);
        ValueTask<CachedBrandGameProviderConfiguration> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);
        string GetExternalRoundId(string roundId, long eventId);
        string GetExternalTransactionId(string trxId, long eventId, string action);
    }

    public class SkywindService : ISkywindService
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        private readonly IBrandOperations _brandOperations;

        public SkywindService(
            IGameProviderOperations gameProviderOperations,
            ITokenProvider tokenProvider,
            IPlayerOperations playerOperations,
            IGameActionQueries gameActionQueries,
            ISettingOperations settingOperations,
            IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper,
            IBrandOperations brandOperations)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        async ValueTask<GameActionExternalIds> ISkywindService.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<GameProviderConfigurationData> ISkywindService.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
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

        async ValueTask<CachedPlayerLoginRecord> ISkywindService.DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            return await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
        }

        async ValueTask<Guid> ISkywindService.GetGameProviderIdAsync()
        {
            return await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Skywind).ConfigureAwait(false);
        }

        async ValueTask<SkywindConfiguration> ISkywindService.GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<SkywindConfiguration>(SettingConsts.GameProviderConfigurations.Skywind);

        async ValueTask<Guid> ISkywindService.GetGameProviderIdByCodeAsync()
                    => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Skywind);

        async ValueTask<string> ISkywindService.GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
            => await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(currencyCode, gameProviderId).ConfigureAwait(false);

        async ValueTask<Guid> ISkywindService.GetBrandIdByPlayerCodeAsync(string playerCode)
            => await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

        async ValueTask<CachedBrandGameProviderConfiguration> ISkywindService.GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId)
            => await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

        string ISkywindService.GetExternalRoundId(string roundId, long eventId) => $"{roundId}_{eventId}";

        string ISkywindService.GetExternalTransactionId(string trxId, long eventId, string action)
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

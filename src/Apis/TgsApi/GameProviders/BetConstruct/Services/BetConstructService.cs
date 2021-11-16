using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Services
{
    public interface IBetConstructService
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
            Guid gameProviderId, 
            Guid gameProviderConfigurationId, 
            PlatformType platformType);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(
            Guid gameProviderId, 
            string playerCode, 
            string externalTransactionId);
        ValueTask<BetConstructConfiguration> GetConfigurationAsync();

        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);

        ValueTask<string> GetGameProviderCurrenciesByInternalCodeAsync(
            Guid gameProviderId,string currencyCode);

        ValueTask<Guid> GetGameProviderIdByCodeAsync();
        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

        ValueTask<CachedBrandGameProviderConfiguration> GetBrandGameProviderConfigurationAsync(
            Guid brandId, Guid gameProviderId);
    }

    public class BetConstructService : IBetConstructService
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderQueries _gameProviderQueries;
        private readonly ISettingOperations _settingOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly HttpContext _httpContextAccessor;

        public BetConstructService(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IPlayerOperations playerOperations,
            IGameProviderQueries gameProviderQueries,
            ISettingOperations settingOperations,
            IBrandOperations brandOperations)
        {
            _httpContextAccessor = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameProviderQueries = gameProviderQueries ?? throw new ArgumentNullException(nameof(gameProviderQueries));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        async ValueTask<GameActionExternalIds> IBetConstructService.GetGameActionExternalIdAsync(
            Guid gameProviderId, 
            string playerCode, 
            string externalTransactionId) 
            => await _gameActionQueries
                .GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId)
                .ConfigureAwait(false);

        async ValueTask<GameProviderConfigurationData> IBetConstructService.GetGameProviderConfigurationDataAsync(
            Guid gameProviderId, 
            Guid gameProviderConfigurationId, 
            PlatformType platformType)
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

        async ValueTask<Player> IBetConstructService.GetPlayerByCodeAsync(string playerCode)
            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);


        async ValueTask<Guid> IBetConstructService.GetGameProviderIdAsync()
            => await _gameProviderOperations
                .GetGameProviderIdByCodeAsync(Consts.GameProvider.BetConstruct)
                .ConfigureAwait(false);
        async ValueTask<string> IBetConstructService.GetGameProviderCurrenciesByInternalCodeAsync(
            Guid gameProviderId, string currencyCode)
        {
            var currencyDict = await _gameProviderQueries
                .GetGameProviderCurrenciesByInternalCodeAsync(gameProviderId)
                .ConfigureAwait(false);

            currencyDict.TryGetValue(currencyCode,out var currency);

            return currency;
        }

        async ValueTask<BetConstructConfiguration> IBetConstructService.GetConfigurationAsync() 
            => await _settingOperations.GetValueFromJsonByKeyAsync<BetConstructConfiguration>(
                SettingConsts.GameProviderConfigurations.BetConstruct);
        async ValueTask<Guid> IBetConstructService.GetGameProviderIdByCodeAsync() 
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.BetConstruct);
        async ValueTask<Guid> IBetConstructService.GetBrandIdByPlayerCodeAsync(string playerCode) 
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.BetConstruct);
        async ValueTask<CachedBrandGameProviderConfiguration> IBetConstructService.GetBrandGameProviderConfigurationAsync(
            Guid brandId, 
            Guid gameProviderId) 
            => await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Apis.TgsApi.GameProviders.WGSports.Services
{
    public interface IWGSportsService
    {
        ValueTask<Guid> GetGameProviderIdAsync();
        ValueTask<WGSportsConfiguration> GetConfigurationAsync();
        ValueTask<CachedPlayer> GetCachedPlayerAsync(string playerCode);
        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);
        ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language);
        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);
        ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);
        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);
        string GetGameProviderCode();
        ClientRoundStatus GetClientRound(decimal playerWinLoss);
    }
    public class WGSportsService : IWGSportsService
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        private readonly IBrandOperations _brandOperations;
        public WGSportsService(
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper,
            IBrandOperations brandOperations)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }
        async ValueTask<Guid> IWGSportsService.GetGameProviderIdAsync(
            ) => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.WGSports).ConfigureAwait(false);

        async ValueTask<WGSportsConfiguration> IWGSportsService.GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<WGSportsConfiguration>(SettingConsts.GameProviderConfigurations.WGSports);

        async ValueTask<CachedPlayer> IWGSportsService.GetCachedPlayerAsync(string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                throw new PlayerLoginExpiredException();
            }
            return player;
        }

        async ValueTask<Player> IWGSportsService.GetPlayerByCodeAsync(string playerCode)
            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);

        async ValueTask<string> IWGSportsService.GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language)
            => await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, language).ConfigureAwait(false);

        async ValueTask<string> IWGSportsService.GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
            => await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(currencyCode, gameProviderId).ConfigureAwait(false);

        async ValueTask<Guid> IWGSportsService.GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
            => await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

        async ValueTask<CachedGame> IWGSportsService.GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        string IWGSportsService.GetGameProviderCode()
        {
            return Consts.GameProvider.WGSports;
        }

        ClientRoundStatus IWGSportsService.GetClientRound(decimal playerWinLoss)
           => playerWinLoss switch
           {
               > 0 => ClientRoundStatus.Win,
               <= 0 => ClientRoundStatus.Loss,
           };
    }
}

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

namespace TGS.Apis.TgsApi.GameProviders.WGHorse.Services
{
    public interface IWGHorseService
    {
        ValueTask<Guid> GetGameProviderIdAsync();
        ValueTask<WGHorseConfiguration> GetConfigurationAsync();
        ValueTask<CachedPlayer> GetCachedPlayerAsync(string playerCode);
        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);
        ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language);
        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);
        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);
        string GetGameProviderCode();
        ClientRoundStatus GetClientRound(decimal playerWinLoss);
    }
    public class WGHorseService : IWGHorseService
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        public WGHorseService(
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
        }

        async ValueTask<Guid> IWGHorseService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.WGHorse).ConfigureAwait(false);

        async ValueTask<WGHorseConfiguration> IWGHorseService.GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<WGHorseConfiguration>(SettingConsts.GameProviderConfigurations.WGHorse);

        async ValueTask<CachedPlayer> IWGHorseService.GetCachedPlayerAsync(string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                throw new PlayerLoginExpiredException();
            }
            return player;
        }

        async ValueTask<Player> IWGHorseService.GetPlayerByCodeAsync(string playerCode)
            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);

        async ValueTask<string> IWGHorseService.GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language)
            => await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, language).ConfigureAwait(false);

        async ValueTask<string> IWGHorseService.GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
            => await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(currencyCode, gameProviderId).ConfigureAwait(false);

        async ValueTask<CachedGame> IWGHorseService.GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        string IWGHorseService.GetGameProviderCode()
        {
            return Consts.GameProvider.WGHorse;
        }

        ClientRoundStatus IWGHorseService.GetClientRound(decimal playerWinLoss)
          => playerWinLoss switch
          {
              > 0 => ClientRoundStatus.Win,
              <= 0 => ClientRoundStatus.Loss,
          };
    }
}

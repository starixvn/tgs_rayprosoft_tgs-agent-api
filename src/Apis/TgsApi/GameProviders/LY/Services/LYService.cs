using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.LY;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Apis.TgsApi.GameProviders.LY.Services
{
    public interface ILYService
    {
        ValueTask<LYBrandMerchant> GetBrandMerchantAsync(string brandCode);

        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);

        ValueTask<LYConfiguration> GetConfigurationAsync();

        DateTimeOffset GetDateTimeOffsetTimeZone8();

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);
    }

    public class LYService : ILYService
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public LYService(
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

        public async ValueTask<LYBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<LYConfiguration>(SettingConsts.GameProviderConfigurations.LY);
            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        public async ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        public async ValueTask<LYConfiguration> GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<LYConfiguration>(SettingConsts.GameProviderConfigurations.LY);

        public DateTimeOffset GetDateTimeOffsetTimeZone8()
            => DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

        public string GetGameProviderCode()
            => Consts.GameProvider.LY;

        public async ValueTask<Guid> GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.LY).ConfigureAwait(false);

        public async ValueTask<Player> GetPlayerByCodeAsync(string playerCode)
            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);
    }
}

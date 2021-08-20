using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.KY;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public interface IKYService
    {
        ValueTask<Guid> GetGameProviderIdAsync();
        ValueTask<KYConfiguration> GetConfigurationAsync();
        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);
        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);
        ValueTask<KYBrandMerchant> GetBrandMerchant(string brandCode);
        string GetGameProviderCode();
        ClientRoundStatus GetClientRound(decimal playerWinLoss);
    }
    public class KYService : IKYService
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        public KYService(
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

        async ValueTask<KYBrandMerchant> IKYService.GetBrandMerchant(string brandCode)
        {
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts.GameProviderConfigurations.KY);
            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        async ValueTask<CachedGame> IKYService.GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        ClientRoundStatus IKYService.GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => ClientRoundStatus.Win,
                < 0 => ClientRoundStatus.Loss,
                0 => ClientRoundStatus.Draw
            };


        async ValueTask<KYConfiguration> IKYService.GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts.GameProviderConfigurations.KY);

        string IKYService.GetGameProviderCode()
            => Consts.GameProvider.KY;

        async ValueTask<Guid> IKYService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.KY).ConfigureAwait(false);

        async ValueTask<Player> IKYService.GetPlayerByCodeAsync(string playerCode)
            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);
    }
}

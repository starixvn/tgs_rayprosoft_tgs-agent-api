using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.VR;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Apis.TgsApi.GameProviders.VR.Services
{
    public interface IVRService
    {
        ValueTask<Guid> GetGameProviderIdAsync();
        ValueTask<VRConfiguration> GetConfigurationAsync();
        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);
        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);
        ValueTask<VRBrandMerchant> GetBrandMerchant(string brandCode);
        string GetGameProviderCode();
        RoundStatus GetRoundStatus(int state);
        ClientRoundStatus GetClientRound(int state);
    }
    public class VRService : IVRService
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
        public VRService(
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

        async ValueTask<VRBrandMerchant> IVRService.GetBrandMerchant(string brandCode)
        {
            var configuration =
              await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                  .GameProviderConfigurations.VR);
            var brandMerchant = configuration.BrandMerchants
                 .Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants
                    .Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        async ValueTask<CachedGame> IVRService.GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        RoundStatus IVRService.GetRoundStatus(int state) =>
           state switch
           {
               1 => RoundStatus.Closed,
               2 => RoundStatus.Closed,
               3 => RoundStatus.Closed,
               _ => RoundStatus.Open
           };

        ClientRoundStatus IVRService.GetClientRound(int state) =>
           state switch
           {
               1 => ClientRoundStatus.Void,
               2 => ClientRoundStatus.Loss,
               3 => ClientRoundStatus.Win,
               _ => ClientRoundStatus.Open
           };

        async ValueTask<VRConfiguration> IVRService.GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts.GameProviderConfigurations.VR);

        string IVRService.GetGameProviderCode()
            => Consts.GameProvider.VR;

        async ValueTask<Guid> IVRService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.VR).ConfigureAwait(false);

        async ValueTask<Player> IVRService.GetPlayerByCodeAsync(string playerCode)
            => await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);
    }
}

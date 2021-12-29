using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.TF;

namespace TGS.Apis.TgsApi.GameProviders.TF.Services
{
    public sealed class TFGameInitializer : IGameInitializer
    {
        private readonly ITFApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly ITFService _service;
        private readonly ISettingOperations _settingOperations;

        public TFGameInitializer(
            IPlayerOperations playerOperations,
            ITFService service,
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            ITFApiClient client)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations
                .GetCachedPlayerAsync(request.Token.PlayerCode)
                .ConfigureAwait(false);

            var gameProviderId = await _service
                .GetGameProviderIdAsync()
                .ConfigureAwait(false);

            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<TFConfiguration>(SettingConsts.GameProviderConfigurations.TFGaming)
                .ConfigureAwait(false);

            var brandCode = player.BrandCode;
            var currencyCode = player.DefaultWalletCurrencyCode;

            var brandMerchant = configuration.BrandMerchants
                .Where(brand => brand.BrandCode == player.BrandCode && brand.BrandCurrency == currencyCode)
                .FirstOrDefault();

            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants
                    .Where(brand => brand.BrandCode == "Default" && brand.BrandCurrency == currencyCode)
                    .FirstOrDefault();
            }

            var url = await _client
                .GetGameUrlAsync(brandMerchant.LaunchUrl, brandMerchant.PublicToken, player.PlayerCode)
                .ConfigureAwait(false);

            return new GetGameUrlResponse
            {
                Url = url
            };
        }
    }
}

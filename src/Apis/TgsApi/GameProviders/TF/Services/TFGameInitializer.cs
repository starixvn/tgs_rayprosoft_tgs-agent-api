using System;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.PluggableServices.Models;
using System.Linq;
using TGS.Shared.Game.ApiClients.TF;

namespace TGS.Apis.TgsApi.GameProviders.TF.Services
{
    public sealed class TFGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly ITFApiClient _client;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ITFService _service;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public TFGameInitializer(
            IBrandGameProviderOperations brandGameProviderOperations,
            IPlayerOperations playerOperations,
            ITFService service,
            ISettingOperations settingOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper,
            ITokenProvider tokenProvider,
            IGameProviderOperations gameProviderOperations,
            IBrandOperations brandOperations,
            ITFApiClient client)
        {
            _brandGameProviderOperations = brandGameProviderOperations
                ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
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

            var currencyCode = await _service
                .GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode)
                .ConfigureAwait(false);

            var brandMerchant = configuration.BrandMerchants
                .Where(brand => brand.BrandCurrency == currencyCode)
                .First();            

            var url = await _client
                .GetGameUrlAsync(configuration.LaunchUrl, brandMerchant.PublicToken, player.PlayerCode)
                .ConfigureAwait(false);

            return new GetGameUrlResponse
            {                
                Url = url
            };
        }
    }
}

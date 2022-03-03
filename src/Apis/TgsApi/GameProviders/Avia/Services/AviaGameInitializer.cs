using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.Avia;

namespace TGS.Apis.TgsApi.GameProviders.Avia.Services
{
    public record AviaGameInitializer : IGameInitializer
    {
        private readonly IAviaApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly IAviaService _service;

        public AviaGameInitializer(
            IAviaApiClient client,
            IPlayerOperations playerOperations,
            IAviaService service
        )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _service.GetBrandMerchantAsync(player.BrandCode).ConfigureAwait(false);
            var cultureCode = await _service.GetGameProviderCultureCodeAsync(gameProviderId, player.BrandId, request.Language).ConfigureAwait(false);
            var currencyCode = await _service.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            (bool IsPlayer, Uri GameUrl) = await _client.GetGameUrlAsync(
               brandMerchant.ApiUrl,
               brandMerchant.Authorization,
               player.PlayerCode,
               cultureCode,
               null,
               null).ConfigureAwait(false);

            if (!IsPlayer)
            {
                IsPlayer = await _client.RegisterAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Authorization,
                player.PlayerCode,
                brandMerchant.PlayerPassword,
                currencyCode).ConfigureAwait(false);

                if (IsPlayer)
                {
                    (IsPlayer, GameUrl) = await _client.GetGameUrlAsync(
                        brandMerchant.ApiUrl,
                        brandMerchant.Authorization,
                        player.PlayerCode,
                        cultureCode,
                        null,
                        null).ConfigureAwait(false);
                }
            }

            return new()
            {
                Url = GameUrl.ToString()
            };
        }
    }
}

using System;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.Models.BigGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public class BigGamingGameInitializer : IGameInitializer
    {

        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IBigGamingApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBigGamingGamePlayerCreate _bigGamingGamePlayerCreate;
        private readonly IBigGamingServices _services;

        public BigGamingGameInitializer(
            IBigGamingApiClient client,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IBigGamingGamePlayerCreate bigGamingGamePlayerCreate,
            IBigGamingServices services
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _bigGamingGamePlayerCreate = bigGamingGamePlayerCreate ?? throw new ArgumentNullException(nameof(bigGamingGamePlayerCreate));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _services.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _services.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);
            var brandId = player.BrandId;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, request.Language);
            var cachedGame = await _services.GetCachedGameAsync(request.GameId).ConfigureAwait(false);

            var responseGameUrl = await _services.GetGameUrlAsync(request, brandMerchant, player, cachedGame, cultureCode).ConfigureAwait(false);
            string url;
            if (responseGameUrl.Error != null)
            {
                if (responseGameUrl.Error.Code == "2213")
                {
                    await _bigGamingGamePlayerCreate.CreatePlayerAsync(player);

                    responseGameUrl = await _services.GetGameUrlAsync(request, brandMerchant, player, cachedGame, cultureCode).ConfigureAwait(false);

                    if (responseGameUrl.Result != null)
                    {
                        url = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(responseGameUrl.Result));
                    }
                    else
                    {
                        throw new GameProviderApiErrorException(responseGameUrl.Error.Message);
                    }
                }
                else
                {
                    throw new GameProviderApiErrorException(responseGameUrl.Error.Message);
                }
            }
            else
            {
                url = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(responseGameUrl.Result));
            }
            return new() { Url = url };
        }
    }
}

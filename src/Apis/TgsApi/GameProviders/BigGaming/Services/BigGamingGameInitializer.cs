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
        private readonly IBigGamingServices _bigGamingServices;

        public BigGamingGameInitializer(
            IBigGamingApiClient client,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IBigGamingGamePlayerCreate bigGamingGamePlayerCreate,
            IBigGamingServices bigGamingServices
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _bigGamingGamePlayerCreate = bigGamingGamePlayerCreate ?? throw new ArgumentNullException(nameof(bigGamingGamePlayerCreate));
            _bigGamingServices = bigGamingServices ?? throw new ArgumentNullException(nameof(bigGamingServices));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _bigGamingServices.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);
            var brandId = player.BrandId;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, request.Language);
            /*
            var data = new BigGamingGetGameUrlRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = player.PlayerCode,
                Locale = cultureCode,
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = string.Empty,

            };
            */
            var data = new BigGamingGetGameUrlFishRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = player.PlayerCode,
                Locale = cultureCode,
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = string.Empty,
                GameType = "411"

            };
            var url = default(string);
            var responseGameUrl = await _client.GetGameUrlAsync(brandMerchant, data).ConfigureAwait(false);
            if (responseGameUrl.Error != null)
            {

                if (responseGameUrl.Error.Code == "2213")
                {
                    await _bigGamingGamePlayerCreate.CreatePlayerAsync(player);

                    data = new BigGamingGetGameUrlFishRequest()
                    {
                        Random = GuidExtensions.NewSequentialGuid().ToString(),
                        Sn = brandMerchant.SecretCode,
                        LoginId = player.PlayerCode,
                        Locale = cultureCode,
                        IsMobileUrl = 0,
                        IsHttpsUrl = 0,
                        ReturnUrl = string.Empty,
                        FromIp = string.Empty,
                        GameType = "411"

                    };

                    responseGameUrl = await _client.GetGameUrlAsync(brandMerchant, data).ConfigureAwait(false);
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

using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.WGSports;
using TGS.Shared.Game.Models.WGSports.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.WGSports.Services
{
    public class WGSportsGameInitializer : IGameInitializer
    {
        private readonly IWGSportsApiClient _client;
        private readonly IWGSportsService _service;

        public WGSportsGameInitializer(
            IWGSportsApiClient client,
            IWGSportsService service
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _service.GetCachedPlayerAsync(request.Token.PlayerCode).ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync();
            var configuration = await _service.GetConfigurationAsync();
            var cultureCode = await _service.GetGameProviderCultureCodeAsync(gameProviderId, player.BrandId, request.Language);
            var currencyCode = await _service.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode);

            var prepareLoginRequest = new WGSportsPrepareLoginRequest()
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                PlayerCode = player.PlayerCode,
                PlayerNickname = player.PlayerCode,
                CurrencyCode = currencyCode,
                Handicap = "C"
            };

            var prepareLoginResponse = await _client.GetPrepareLoginAsync(configuration, prepareLoginRequest).ConfigureAwait(false);

            if (prepareLoginResponse.ReturnCode != 1)
            {
                throw new GameProviderApiErrorException(prepareLoginResponse.ReturnCode.ToString());
            }


            var sportsLoginRequest = new WGSportsLoginRequest()
            {
                Token = prepareLoginResponse.Token,
                Lang = cultureCode
            };

            var loginResponse = await _client.GetLoginAsync(configuration, sportsLoginRequest).ConfigureAwait(false);

            if (loginResponse.ReturnCode != 1)
            {
                throw new GameProviderApiErrorException(prepareLoginResponse.ReturnCode.ToString());
            }

            return new() { Url = loginResponse.LoginUrl };
        }
    }
}

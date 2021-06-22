using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.Skywind;
using TGS.Shared.Game.Models.Skywind.Outgoing;
using TGS.Shared.Game.Models.Skywind.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Infrastructures.Security;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public sealed class SkywindGameInitializer : IGameInitializer
    {
        private readonly ISkywindApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISkywindServices _services;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public SkywindGameInitializer(
            ISkywindApiClient client,
            IPlayerOperations playerOperations,
            ISkywindServices services,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations,
            ITokenProvider tokenProvider,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderOperations gameProviderOperations
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _services.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandId = player.BrandId;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, request.Language);
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<SkywindConfiguration>(SettingConsts.GameProviderConfigurations.Skywind);

            var url = default(string);
            if (request.GameId == 0)
            {
                url = $"{configuration.LiveLobbyUrl}#/ticket/{encryptedToken}?lang={cultureCode}";
            }
            else
            {
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId).ConfigureAwait(false);

                var data = new SkywindGameUrlRequest()
                {
                    PlayerCode = player.PlayerCode,
                    GameCode = cachedGame.ExternalId,
                    Ticket = encryptedToken,
                    Language = cultureCode
                };

                var responseGameUrl = await _client.GetGameUrlAsync(configuration, data).ConfigureAwait(false);
                if (responseGameUrl.Code.ToString().IgnoreCaseNotEquals(((int)GameApiError.Success).ToString()))
                {
                    throw new GameProviderApiErrorException(_jsonSerializationProvider.SerializeToString(responseGameUrl));
                }
                else
                {
                    url = responseGameUrl.Url;
                }
            }
            return new() { Url = url };
        }
    }
}
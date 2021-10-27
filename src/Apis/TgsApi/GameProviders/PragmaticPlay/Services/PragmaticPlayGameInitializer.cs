using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.PragmaticPlay;
using TGS.Shared.Game.Models.PragmaticPlay.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public sealed class PragmaticPlayGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IPragmaticPlayApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPragmaticPlayHashProvider _hashProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public PragmaticPlayGameInitializer(
            ITokenProvider tokenProvider,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IPragmaticPlayHashProvider hashProvider,
            IPragmaticPlayApiClient client)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));

            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));

            _brandGameProviderOperations = brandGameProviderOperations ??
                throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);

            var player = await _playerOperations.GetCachedPlayerAsync(token.PlayerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var brandId = player.BrandId;

            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(
                gameProviderId,
                brandId,
                request.Language);

            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<PragmaticPlayConfiguration>(SettingConsts
                    .GameProviderConfigurations.PragmaticPlay);

            var isMobile = token.PlatformType == PlatformType.Mobile;

            var lobbyUrl = default(string);

            var extendedData = request.ExtendedData;

            if (extendedData != null && extendedData.RootElement.TryGetProperty("lobbyurl", out var prop))
            {
                lobbyUrl = prop.GetString();
            }

            var model = new GameUrlRequest
            {
                SecureLogin = configuration.SecureLogin,
                Language = cultureCode,
                ExternalGameId = game.ExternalId,
                Token = encryptedToken,
                Platform = isMobile ? "MOBILE" : "WEB",
                LobbyUrl = lobbyUrl
            };

            var hash = _hashProvider.ComputeHash(model, configuration.SecretKey);

            var gameUrlRequest = model with
            {
                Hash = hash
            };

            var response = await _client.GetGameUrlAsync(configuration.ApiUrl, gameUrlRequest)
                .ConfigureAwait(false);

            if (response.Error.IgnoreCaseNotEquals("0"))
            {
                throw new GameProviderApiErrorException($"Get pragmatic play game url fail: {response.Description}");
            }

            return new()
            {
                Url = response.Url
            };
        }
    }
}
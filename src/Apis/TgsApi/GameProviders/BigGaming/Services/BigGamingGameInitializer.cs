using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
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
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IBigGamingGamePlayerCreate _bigGamingGamePlayerCreate;

        public BigGamingGameInitializer(
            IBigGamingApiClient client,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderOperations gameProviderOperations,
            IHashProvider _hashProvider,
            IBigGamingGamePlayerCreate bigGamingGamePlayerCreate
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            this._hashProvider = _hashProvider ?? throw new ArgumentNullException(nameof(_hashProvider));
            _bigGamingGamePlayerCreate = bigGamingGamePlayerCreate ?? throw new ArgumentNullException(nameof(bigGamingGamePlayerCreate));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                    .GameProviderConfigurations.BigGaming);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode).ConfigureAwait(false);
            var brandId = player.BrandId;

            var externalID = 0;
            if (request.GameId > 0)
            {
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId).ConfigureAwait(false);
                if (cachedGame != null)
                {
                    externalID = Convert.ToInt32(cachedGame.ExternalId);
                }
            }

            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, request.Language);
            var random = GuidExtensions.NewSequentialGuid().ToString();
            var digest = _hashProvider.Hash($"{random }{configuration.SecretCode}{player.PlayerCode}{BigGamingUtility.GetSecretCode(configuration.AgentLoginIdPassword)}", HashAlgorithmKind.Md5);
            var data = new BGGetGameUrlRequest()
            {
                Random = random,
                Digest = digest,
                Sn = configuration.SecretCode,
                LoginId = player.PlayerCode,
                Locale = cultureCode,
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = string.Empty,

            };
            var url = default(string);
            var responseGameUrl = await _client.GetGameUrlAsync(configuration.ApiUrl, data).ConfigureAwait(false);
            if (responseGameUrl.Error != null)
            {
                if (responseGameUrl.Error.Code == "2213")
                {
                    await _bigGamingGamePlayerCreate.CreatePlayerAsync(player);
                    random = GuidExtensions.NewSequentialGuid().ToString();
                    digest = _hashProvider.Hash($"{random }{configuration.SecretCode}{player.PlayerCode}{BigGamingUtility.GetSecretCode(configuration.AgentLoginIdPassword)}", HashAlgorithmKind.Md5);
                    data = new BGGetGameUrlRequest()
                    {
                        Random = random,
                        Digest = digest,
                        Sn = configuration.SecretCode,
                        LoginId = player.PlayerCode,
                        Locale = cultureCode,
                        IsMobileUrl = 0,
                        IsHttpsUrl = 0,
                        ReturnUrl = string.Empty,
                        FromIp = string.Empty,

                    };

                    responseGameUrl = await _client.GetGameUrlAsync(configuration.ApiUrl, data).ConfigureAwait(false);
                    if (responseGameUrl.Result != null)
                    {
                        url = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(responseGameUrl.Result));
                    }
                    else
                    {
                        throw new GameProviderApiErrorException(responseGameUrl.Error.Message);
                    }
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

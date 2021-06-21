using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Shared.Game.Models.Skywind.Outgoing;

namespace TGS.Shared.Game.ApiClients.Skywind
{
    public interface ISkywindApiClient
    {
        ValueTask<SkywindGameUrlResponse> GetGameUrlAsync(SkywindConfiguration configuration, SkywindGameUrlRequest request);
    }

    public sealed class SkywindApiClient : BaseApiClient, ISkywindApiClient
    {
        private static readonly string _accessTokenCacheType = "AccessToken";
        private readonly string _gameProvider = Consts.GameProvider.Skywind;
        private readonly IRedisCacheManager _redisCacheManager;
        public SkywindApiClient(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json,
            IRedisCacheManager redisCacheManager
            ) : base(client, logger, json)
        {
            _redisCacheManager = redisCacheManager ?? throw new ArgumentNullException(nameof(redisCacheManager));
        }

        async ValueTask<SkywindGameUrlResponse> ISkywindApiClient.GetGameUrlAsync(SkywindConfiguration configuration, SkywindGameUrlRequest request)
        {
            var path = $"/players/{request.PlayerCode}/games/{request.GameCode}?ticket={request.Ticket}&language{request.Language}";
            var getaccessToken = await GetAccessTokenAsync(configuration).ConfigureAwait(false);

            var heard = new Dictionary<string, string> {
             { "X-ACCESS-TOKEN", getaccessToken }
            };


            return await ExecuteJsonRequestAsync<SkywindGameUrl, SkywindGameUrlResponse>(HttpMethod.Get, configuration.ApiUrl, path, null, heard, logPrefix: $"{_gameProvider}.UserLogin").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the user login asynchronous.
        /// AccessToken 有效時間:一個小时了
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private async ValueTask<string> GetAccessTokenAsync(SkywindConfiguration configuration)
        {
            var accessToken = default(string);

            accessToken = await _redisCacheManager.SafeGetAsync(_gameProvider, _accessTokenCacheType, () => Task.FromResult(default(string))).ConfigureAwait(false);

            if (string.IsNullOrEmpty(accessToken))
            {
                var source = new SkywindUserLoginRequest()
                {
                    SecretKey = configuration.SecretKey,
                    Username = configuration.Username,
                    Password = configuration.Password
                };
                var response = await ExecuteJsonRequestAsync<SkywindUserLoginRequest, SkywindUserLoginResponse>(HttpMethod.Post, configuration.ApiUrl, " /login", source, logPrefix: $"{_gameProvider}.UserLogin").ConfigureAwait(false);
                if (!string.IsNullOrEmpty(response.AccessToken))
                {
                    await _redisCacheManager.SafeCacheAsync(_gameProvider, _accessTokenCacheType, TimeSpan.FromMinutes(45), response.AccessToken).ConfigureAwait(false);
                    accessToken = response.AccessToken;
                }
            }
            return accessToken;
        }
    }
}

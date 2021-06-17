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
        private static readonly string _accessTokenCacheType = $"{ Consts.GameProvider.Skywind}:AccessToken";
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
            var path = $"/players/{request.PlayerCode}/games/{request.GameCode}?ticket={request.Ticket}";
            var skywindUserLoginResponse = await GetUserLoginAsync(configuration).ConfigureAwait(false);

            var heard = new Dictionary<string, string> {
             { "X-ACCESS-TOKEN", skywindUserLoginResponse.AccessToken }
            };


            return await ExecuteJsonRequestAsync<SkywindGameUrl, SkywindGameUrlResponse>(HttpMethod.Get, configuration.ApiUrl, path, null, heard, logPrefix: $"{_gameProvider}.UserLogin").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the user login asynchronous.
        /// AccessToken 有效時間:一個小时了
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private async ValueTask<SkywindUserLoginResponse> GetUserLoginAsync(SkywindConfiguration configuration)
        {
            var cacheKey = _accessTokenCacheType;

            var response = default(SkywindUserLoginResponse);

            response = await _redisCacheManager.SafeGetAsync(_accessTokenCacheType, cacheKey, () => Task.FromResult(default(SkywindUserLoginResponse))).ConfigureAwait(false);

            if (response == null)
            {
                var source = new SkywindUserLoginRequest()
                {
                    SecretKey = configuration.SecretKey,
                    Username = configuration.Username,
                    Password = configuration.Password
                };
                response = await ExecuteJsonRequestAsync<SkywindUserLoginRequest, SkywindUserLoginResponse>(HttpMethod.Post, configuration.ApiUrl, " /login", source, logPrefix: $"{_gameProvider}.UserLogin").ConfigureAwait(false);
                if (!string.IsNullOrEmpty(response.AccessToken))
                {
                    await _redisCacheManager.SafeCacheAsync(_accessTokenCacheType, cacheKey, TimeSpan.FromMinutes(45), response).ConfigureAwait(false);
                }
            }
            return response;
        }
    }
}

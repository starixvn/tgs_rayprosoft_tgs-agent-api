using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
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

        private readonly string _gameProvider = Consts.GameProvider.SexyGaming;

        public SkywindApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
           : base(client, logger, json)
        {
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

        private ValueTask<SkywindUserLoginResponse> GetUserLoginAsync(SkywindConfiguration configuration)
        {
            var source = new SkywindUserLoginRequest()
            {
                SecretKey = configuration.SecretKey,
                Username = configuration.Username,
                Password = configuration.Password
            };
            return ExecuteJsonRequestAsync<SkywindUserLoginRequest, SkywindUserLoginResponse>(HttpMethod.Post, configuration.ApiUrl, " /login", source, logPrefix: $"{_gameProvider}.UserLogin");
        }
    }
}

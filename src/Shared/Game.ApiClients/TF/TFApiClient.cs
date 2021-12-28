using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.TF;

namespace TGS.Shared.Game.ApiClients.TF
{
    internal class TFApiClient : BaseApiClient, ITFApiClient
    {
        private readonly HttpClient _client;
        private readonly IApiClientLogger _logger;
        private readonly ITokenProvider _tokenProvider;

        public TFApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json, ITokenProvider tokenProvider)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        public async ValueTask<string> GetGameUrlAsync(Uri apiUrl, string publicToken, string playerCode)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{apiUrl.OriginalString}/api/v4/launch/url/?auth={publicToken}"),
                Content = new StringContent("", Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            var response = await _client
                .SendAsync(request)
                .ConfigureAwait(false);

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<LaunchResponse>(bodyJson);

            var launchUrl = result.LaunchUrl;

            var token = await _tokenProvider
                .EncryptAsync(playerCode)
                .ConfigureAwait(false);

            return $"{launchUrl}?auth={publicToken}&token={token}";
        }
    }
}

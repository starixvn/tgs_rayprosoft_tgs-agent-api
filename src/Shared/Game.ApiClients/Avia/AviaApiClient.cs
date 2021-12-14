using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.Logging;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Shared.Game.Models.Avia;
using TGS.Shared.Game.Models.Avia.Commons;

namespace TGS.Shared.Game.ApiClients.Avia
{
    internal class AviaApiClient : BaseApiClient, IAviaApiClient
    {
        private readonly HttpClient _client;
        private readonly IApiClientLogger _logger;

        public AviaApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<(bool IsPlayer, Uri? GameUrl)> GetGameUrlAsync(Uri apiUrl, string authorization, string playerCode, string lang, int? cateId, int? matchId)
        {
            var request = new
            {
                UserName = playerCode,
                CateID = cateId,
                MatchID = matchId
            };

            var response = await PostAsync<AviaBaseResponse<AviaLoginResponse>>(
                apiUrl,
                authorization,
                ToFormData(request),
                AviaAction.Login,
                lang
                ).ConfigureAwait(false);

            if (response.Success == 0)
            {
                if (response.Info.Error.Contains("NOUSER"))
                {
                    return (false, null);
                }
                else
                {
                    return (true, null);
                }
            }

            return (true, new(response.Info.Url));
        }

        public async ValueTask<bool> RegisterAsync(Uri apiUrl, string authorization, string playerCode, string password, string currency)
        {
            var request = new
            {
                UserName = playerCode,
                Password = password,
                Currency = currency
            };
            var response = await PostAsync<AviaBaseResponse<AviaErrorResponse>>(
                apiUrl,
                authorization,
                ToFormData(request),
                AviaAction.Register).ConfigureAwait(false);

            return response.Success == 1;
        }

        private async Task<TResponse> PostAsync<TResponse>(Uri apiUrl, string authorization, Dictionary<string, string> parameters, AviaAction action, string contentLanguage = "CHN")
        {
            var requestUri = AviaMapper.AviaActionMapper.Where(x => x.Key == action).First().Value;
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResponse);

            var content = new FormUrlEncodedContent(parameters);
            content.Headers.ContentType.CharSet = "utf-8";
            content.Headers.Add("Content-Language", contentLanguage);
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Post;
            requestMessage.RequestUri = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{requestUri}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorization);

            requestMessage.Content = content;
            var httpResponseMessage = await _client.SendAsync(requestMessage);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _logger.LogHttpResponse(new ApiLogEvent
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = $"{Consts.GameProvider.Avia.ToLower()}.{action}",
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(parameters),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResponse>(responseMessage);
            }

            return response;
        }

        private Dictionary<string, string> ToFormData<TData>(TData request)
        {
            var value = JsonSerializer.Serialize(request);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(value);
            // return string.Join("&", items.Where(x => x.Value != null).Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }
    }
}

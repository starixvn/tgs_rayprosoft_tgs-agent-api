using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.ApiClients.TCG
{
    public interface ITCGApiClient
    {

    }
    public class TCGApiClient : BaseApiClient, ITCGApiClient
    {
        private static readonly string GameProviderCode = Consts.GameProvider.TCG;

        private readonly HttpClient _client;

        private readonly IApiClientLogger _logger;

        private readonly IHashProvider _hashProvider;

        private readonly IJsonSerializationProvider _json;

        public TCGApiClient(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json,
            IHashProvider hashProvider)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
        }

        public async ValueTask<TResponse> GetAPIAsync<TRequest, TResponse>(Uri apiUrl, TRequest request) where TRequest : TCGBaseRequest where TResponse : TCGBaseResponse
        {

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResponse);
            var dataJson = DESCrypt(_json.SerializeToString(request));
            var data = "merchant_code=" + HttpUtility.UrlEncode("test")
                  + "&params=" + HttpUtility.UrlEncode(dataJson)
                + "&sign=" + HttpUtility.UrlEncode(She256("aa123456", dataJson));
            HttpContent content = new StringContent(data, Encoding.UTF8);
            var httpResponseMessage = await _client.PostAsync(apiUrl, content);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _logger.LogHttpResponse(new ApiLogEvent
            {
                ip = string.Empty,
                path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                prefix = GameProviderCode.ToLower(),
                servername = Environment.MachineName.NoLongerThan(128),
                requestheaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                requestcontent = _json.SerializeToString(request),
                responseheaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                responsecontent = responseMessage,
                responsecode = (int)httpResponseMessage.StatusCode,
                executiontimems = (int)sw.ElapsedMilliseconds,
                recordedon = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = _json.DeserializeFromString<TResponse>(responseMessage);
            }

            return response;
        }

        private string DESCrypt(string json)
        {
            return HttpUtility.HtmlEncode(json);
        }

        private string She256(string key, string encryptedParams)
        {
            var datas = Encoding.UTF8.GetBytes(encryptedParams);
            var encodingKey = Encoding.UTF8.GetBytes(key);
            var base64Str = _hashProvider.Hash(datas, encodingKey, HashAlgorithmKind.Sha256);

            return HttpUtility.UrlEncode(base64Str);
        }
    }
}

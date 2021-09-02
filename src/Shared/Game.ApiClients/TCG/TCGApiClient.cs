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
using TGS.Shared.Game.Models.TCG.Outgoing;

namespace TGS.Shared.Game.ApiClients.TCG
{
    public interface ITCGApiClient
    {

    }
    public class TCGApiClient : BaseApiClient, ITCGApiClient
    {
        private readonly HttpClient _client;

        private readonly IApiClientLogger _logger;

        private readonly IJsonSerializationProvider _json;

        private readonly ITCGUtility _utility;

        public TCGApiClient(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json,
            ITCGUtility utility)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<TCGRegisterPlayerResponse> RegisterPlayerAsync(TCGRegisterPlayerRequest request)
        => GetAPIAsync<TCGRegisterPlayerRequest, TCGRegisterPlayerResponse>(new("http://www.connect6play.com/doBusiness.do"), request, "RegisterPlayer");


        private async ValueTask<TResponse> GetAPIAsync<TRequest, TResponse>(Uri apiUrl, TRequest request, string prefix) where TRequest : TCGBaseRequest where TResponse : TCGBaseResponse
        {

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResponse);
            var jsonString = _json.SerializeToString(request);
            var encryptedParams = _utility.DESCEncrypt(jsonString, "EuWEfLac");
            var sign = _utility.GetHash256(encryptedParams, "h2f69FzdSmMZzrdE");
            var data =
                "merchant_code=" + HttpUtility.UrlEncode("pegacnys")
                + "&params=" + HttpUtility.UrlEncode(encryptedParams)
                + "&sign=" + HttpUtility.UrlEncode(sign);

            HttpContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

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
                prefix = _utility.GetPrefix(prefix),
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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Models;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Incoming;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Shared.Game.ApiClients.KY
{
    public interface IKYApiClient
    {
        ValueTask<KYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, KYApiRequest request);
        ValueTask<KYGPGameRecordResponse> GameRecordAsync(Uri apiUrl, Models.KY.Incoming.KYGPGameRecordRequest request);

    }

    public sealed class KYApiClient : BaseApiClient, IKYApiClient
    {
        private static readonly string GameProviderCode = Consts.GameProvider.KY;

        private readonly HttpClient _client;

        private readonly IApiClientLogger _logger;

        public KYApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async ValueTask<KYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, KYApiRequest request)
        {

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(KYApiResponse<TData>);

            var paths = KYUtility.GetPath(request);
            var url = $"{apiUrl?.ToString()}?{paths}";

            var httpResponseMessage = await _client.GetAsync(url);
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
                requestcontent = JsonSerializer.Serialize(request),
                responseheaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                responsecontent = responseMessage,
                responsecode = (int)httpResponseMessage.StatusCode,
                executiontimems = (int)sw.ElapsedMilliseconds,
                recordedon = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<KYApiResponse<TData>>(responseMessage);
            }

            return response;
        }

        ValueTask<KYGPGameRecordResponse> IKYApiClient.GameRecordAsync(Uri apiUrl, KYGPGameRecordRequest request) =>
            ExecuteJsonRequestAsync<Models.KY.Incoming.KYGPGameRecordRequest, KYGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/ky/GameRecord", request);

    }
}

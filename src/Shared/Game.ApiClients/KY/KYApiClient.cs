using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
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
        private readonly HttpClient _client;

        public KYApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _client = client;
        }

        public async ValueTask<KYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, KYApiRequest request)
        {

            var paths = KYUtility.GetPath(request);

            var url = $"{apiUrl?.ToString()}?{paths}";

            var httpResponseMessage = await _client.GetAsync(url);

            var response = default(KYApiResponse<TData>);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
                {
                    var Content = await sr.ReadToEndAsync() ?? string.Empty;
                    response = JsonSerializer.Deserialize<KYApiResponse<TData>>(Content);
                }
            }

            return response;
        }

        ValueTask<KYGPGameRecordResponse> IKYApiClient.GameRecordAsync(Uri apiUrl, KYGPGameRecordRequest request) =>
            ExecuteJsonRequestAsync<Models.KY.Incoming.KYGPGameRecordRequest, KYGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/ky/GameRecord", request);

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.GameProviders.Settings.VR;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.VR.Commons;
using TGS.Shared.Game.Models.VR.Incoming;
using TGS.Shared.Game.Models.VR.Outgoing;

namespace TGS.Shared.Game.ApiClients.VR
{
    public interface IVRApiClient
    {
        ValueTask<ResultData> GetAPIAsync<SourceData, ResultData>(VRBrandMerchant brandMerchant, GPAction action, SourceData request);
        Uri GetGameURLAsync(VRBrandMerchant brandMerchant, VRGameUrlRequest request);
        ValueTask<VRGPGameRecordResponse> GameRecordAsync(Uri apiUrl, VRGPGameRecordRequest request);

    }

    public sealed class VRApiClient : BaseApiClient, IVRApiClient
    {
        private readonly HttpClient _client;

        public VRApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        Uri IVRApiClient.GetGameURLAsync(VRBrandMerchant brandMerchant, VRGameUrlRequest request)
        {
            var path = VRUtility.GetPath(request);

            var dataEncrypt = VRUtility.AesEncrypt(path, brandMerchant.Deskey, true);

            var apiUrl = brandMerchant.ApiUrl;

            var data = new VRApiRequest()
            {
                version = brandMerchant.Version,
                id = brandMerchant.Merchantid,
                data = dataEncrypt,
            };

            return new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{VRUtility.GetAction(GPAction.GetGameUrl)}?{VRUtility.GetPath(data)}");

        }

        async ValueTask<ResultData> IVRApiClient.GetAPIAsync<SourceData, ResultData>(VRBrandMerchant brandMerchant, GPAction action, SourceData request)
        {
            var dataEncrypt = VRUtility.AesEncrypt(JsonSerializer.Serialize(request), brandMerchant.Deskey, false);

            var apiUrl = brandMerchant.ApiUrl;

            var data = new VRApiRequest()
            {
                version = brandMerchant.Version,
                id = brandMerchant.Merchantid,
                data = dataEncrypt,
            };

            HttpContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var url = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{VRUtility.GetAction(action)}");

            var httpResponseMessage = await _client.PostAsync(url, content);

            var response = default(ResultData);
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
                {
                    var Content = await sr.ReadToEndAsync() ?? string.Empty;
                    var contentDecrypt = VRUtility.AesDecrypt(Content, brandMerchant.Deskey, false);
                    response = JsonSerializer.Deserialize<ResultData>(contentDecrypt);
                }
            }

            return response;
        }

        ValueTask<VRGPGameRecordResponse> IVRApiClient.GameRecordAsync(Uri apiUrl, VRGPGameRecordRequest request) =>
          ExecuteJsonRequestAsync<VRGPGameRecordRequest, VRGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/vr/GameRecord", request);
    }
}

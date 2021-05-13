using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Models;
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
        private static readonly string GameProviderCode = Consts.GameProvider.VR;

        private readonly HttpClient _client;

        private readonly IApiClientLogger _logger;

        public VRApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(ResultData);

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
            sw.Stop();

            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {

                var contentDecrypt = VRUtility.AesDecrypt(responseMessage, brandMerchant.Deskey, false);
                if (!string.IsNullOrEmpty(contentDecrypt))
                {
                    responseMessage = contentDecrypt;
                    response = JsonSerializer.Deserialize<ResultData>(contentDecrypt);
                }

            }

            _logger.LogHttpResponse(new ApiLogEvent
            {
                ip = string.Empty,
                path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                prefix = GameProviderCode.ToLower(),
                servername = Environment.MachineName.NoLongerThan(128),
                requestheaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                requestcontent = JsonSerializer.Serialize(data),
                responseheaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                responsecontent = responseMessage,
                responsecode = (int)httpResponseMessage.StatusCode,
                executiontimems = (int)sw.ElapsedMilliseconds,
                recordedon = DateTimeOffset.UtcNow
            });



            return response;
        }

        ValueTask<VRGPGameRecordResponse> IVRApiClient.GameRecordAsync(Uri apiUrl, VRGPGameRecordRequest request) =>
          ExecuteJsonRequestAsync<VRGPGameRecordRequest, VRGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/vr/GameRecord", request);
    }
}

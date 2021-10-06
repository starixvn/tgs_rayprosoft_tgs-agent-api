using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.Logging;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
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
        ValueTask<VRErrorCodeResponse> CreatePlayerAsync(VRBrandMerchant brandMerchant, VRCreatePlayerRequest request);

        ValueTask<VRFnudResponse> FundAsync(VRBrandMerchant brandMerchant, GPAction action, VRFnudRequest request);

        ValueTask<VRFundCheckingResponse> FundCheckingAsync(VRBrandMerchant brandMerchant, VRFundCheckingRequest request);

        ValueTask<VRGPGameRecordResponse> GameRecordAsync(Uri apiUrl, VRGPGameRecordRequest request);

        ValueTask<VRGetBalanceResponse> GetBalanceAsync(VRBrandMerchant brandMerchant, VRGetBalanceRequest request);

        ValueTask<VRGameRecordResponse> GetGameRecordAsync(VRBrandMerchant brandMerchant, VRGameRecordRequest request);

        Uri GetGameURLAsync(VRBrandMerchant brandMerchant, VRGameUrlRequest request);

        ValueTask<VRKickResponse> KickAsync(VRBrandMerchant brandMerchant, VRKickRequest request);
    }

    public sealed class VRApiClient : BaseApiClient, IVRApiClient
    {
        private readonly HttpClient _client;

        private readonly IApiClientLogger _logger;

        private readonly IVRUtility _utility;

        public VRApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json, IVRUtility utility)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<VRErrorCodeResponse> CreatePlayerAsync(VRBrandMerchant brandMerchant, VRCreatePlayerRequest request)
            => GetAPIAsync<VRCreatePlayerRequest, VRErrorCodeResponse>(brandMerchant, GPAction.CreatePlayer, request);

        public ValueTask<VRFnudResponse> FundAsync(VRBrandMerchant brandMerchant, GPAction action, VRFnudRequest request)
            => GetAPIAsync<VRFnudRequest, VRFnudResponse>(brandMerchant, action, request);

        public ValueTask<VRFundCheckingResponse> FundCheckingAsync(VRBrandMerchant brandMerchant, VRFundCheckingRequest request)
            => GetAPIAsync<VRFundCheckingRequest, VRFundCheckingResponse>(brandMerchant, GPAction.FundChecking, request);

        public ValueTask<VRGPGameRecordResponse> GameRecordAsync(Uri apiUrl, VRGPGameRecordRequest request) =>
          ExecuteJsonRequestAsync<VRGPGameRecordRequest, VRGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/vr/GameRecord", request);

        public ValueTask<VRGetBalanceResponse> GetBalanceAsync(VRBrandMerchant brandMerchant, VRGetBalanceRequest request)
            => GetAPIAsync<VRGetBalanceRequest, VRGetBalanceResponse>(brandMerchant, GPAction.GetBalance, request);

        public ValueTask<VRGameRecordResponse> GetGameRecordAsync(VRBrandMerchant brandMerchant, VRGameRecordRequest request)
            => GetAPIAsync<VRGameRecordRequest, VRGameRecordResponse>(brandMerchant, GPAction.GameRecord, request);

        public Uri GetGameURLAsync(VRBrandMerchant brandMerchant, VRGameUrlRequest request)
        {
            var path = _utility.GetPath(request);

            var dataEncrypt = _utility.AesEncrypt(path, brandMerchant.Deskey, true);

            var apiUrl = brandMerchant.ApiUrl;

            var data = new VRApiRequest()
            {
                version = brandMerchant.Version,
                id = brandMerchant.Merchantid,
                data = dataEncrypt,
            };

            return new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{_utility.GetAction(GPAction.GetGameUrl)}?{_utility.GetPath(data)}");
        }

        public ValueTask<VRKickResponse> KickAsync(VRBrandMerchant brandMerchant, VRKickRequest request)
            => GetAPIAsync<VRKickRequest, VRKickResponse>(brandMerchant, GPAction.Kick, request);

        private async ValueTask<ResultData> GetAPIAsync<SourceData, ResultData>(VRBrandMerchant brandMerchant, GPAction action, SourceData request)
        {
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(ResultData);

            var dataEncrypt = _utility.AesEncrypt(JsonSerializer.Serialize(request), brandMerchant.Deskey, false);

            var apiUrl = brandMerchant.ApiUrl;

            var data = new VRApiRequest()
            {
                version = brandMerchant.Version,
                id = brandMerchant.Merchantid,
                data = dataEncrypt,
            };

            HttpContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var url = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{_utility.GetAction(action)}");

            var httpResponseMessage = await _client.PostAsync(url, content);
            sw.Stop();

            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var contentDecrypt = _utility.AesDecrypt(responseMessage, brandMerchant.Deskey, false);
                if (!string.IsNullOrEmpty(contentDecrypt))
                {
                    responseMessage = contentDecrypt;
                    response = JsonSerializer.Deserialize<ResultData>(contentDecrypt);
                }
            }

            _logger.LogHttpResponse(new ApiLogEvent
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = $"{Consts.GameProvider.VR.ToLower()}.{action.ToString()}",
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(data),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            return response;
        }
    }
}

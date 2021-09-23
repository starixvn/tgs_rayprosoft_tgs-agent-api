using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.Logging;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings.KY;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Incoming;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Shared.Game.ApiClients.KY
{
    public interface IKYApiClient
    {
        ValueTask<KYApiResponse<KYFnudResponse>> FundAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, GPAction action, KYFnudRequest request);

        ValueTask<KYApiResponse<KYFundCheckingResponse>> FundCheckingAsync(KYBrandMerchant brandMerchant, string agentId, KYFundCheckingRequest request);

        ValueTask<KYGPGameRecordResponse> GameRecordAsync(Uri apiUrl, KYGPGameRecordRequest request);

        ValueTask<KYApiResponse<KYGetBalanceResponse>> GetBalanceAsync(KYBrandMerchant brandMerchant, string agentId, KYGetBalanceRequest request);

        ValueTask<KYApiResponse<KYGameRecordResponse>> GetGameRecordAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameRecordRequest request);

        ValueTask<KYApiResponse<KYGameUrlResponse>> GetGameUrlAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameUrlRequest request);

        ValueTask<KYApiResponse<KYKickResponse>> KickAsync(KYBrandMerchant brandMerchant, string agentId, KYKickRequest request);
    }

    public sealed class KYApiClient : BaseApiClient, IKYApiClient
    {
        private readonly HttpClient _client;

        private readonly IApiClientLogger _logger;

        private readonly IKYUtility _utility;

        public KYApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json, IKYUtility utility)
            : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<KYApiResponse<KYFnudResponse>> FundAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, GPAction action, KYFnudRequest request)
        {
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYFnudResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYFundCheckingResponse>> FundCheckingAsync(KYBrandMerchant brandMerchant, string agentId, KYFundCheckingRequest request)
        {
            var action = GPAction.FundChecking;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYFundCheckingResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYGPGameRecordResponse> GameRecordAsync(Uri apiUrl, KYGPGameRecordRequest request) =>
            ExecuteJsonRequestAsync<Models.KY.Incoming.KYGPGameRecordRequest, KYGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/ky/GameRecord", request);

        public ValueTask<KYApiResponse<KYGetBalanceResponse>> GetBalanceAsync(KYBrandMerchant brandMerchant, string agentId, KYGetBalanceRequest request)
        {
            var action = GPAction.GetBalance;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGetBalanceResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYGameRecordResponse>> GetGameRecordAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameRecordRequest request)
        {
            var action = GPAction.GameRecord;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGameRecordResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYGameUrlResponse>> GetGameUrlAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameUrlRequest request)
        {
            var action = GPAction.GetGameUrl;
            request.Action = _utility.GetAction(GPAction.GetGameUrl) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYGameUrlResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        public ValueTask<KYApiResponse<KYKickResponse>> KickAsync(KYBrandMerchant brandMerchant, string agentId, KYKickRequest request)
        {
            var action = GPAction.Kick;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _utility.HashMD5(plainText);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<KYKickResponse>(brandMerchant.ApiUrl, action, apiKYRequest);
        }

        private async ValueTask<KYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, GPAction action, KYApiRequest request)
        {
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(KYApiResponse<TData>);

            var paths = _utility.GetPath(request);
            var url = $"{apiUrl?.ToString()}?{paths}";

            var httpResponseMessage = await _client.GetAsync(url);
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
                Prefix = $"{Consts.GameProvider.KY.ToLower()}.{action.ToString()}",
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(request),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<KYApiResponse<TData>>(responseMessage);
            }

            return response;
        }
    }
}

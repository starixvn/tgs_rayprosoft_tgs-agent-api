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
using TGS.Shared.Game.Models.LY.Commons;
using TGS.Shared.Game.Models.LY.Enums;
using TGS.Shared.Game.Models.LY.Outgoing;

namespace TGS.Shared.Game.ApiClients.LY
{
    internal class LYApiClient : BaseApiClient, ILYApiClient
    {
        private readonly HttpClient _client;
        private readonly IApiClientLogger _logger;
        private readonly ILYUtility _utility;

        public LYApiClient(
            HttpClient client,
           IApiClientLogger logger,
           IJsonSerializationProvider json,
           ILYUtility utility)
             : base(client, logger, json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        public ValueTask<LYApiResponse<LYFnudResponse>> FundAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYAction action, LYFnudRequest request)
        {
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYFnudResponse>(apiUrl, action, apiLYRequest, "Fund");
        }

        public ValueTask<LYApiResponse<LYFundCheckingResponse>> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYFundCheckingRequest request)
        {
            var action = LYAction.FundChecking;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYFundCheckingResponse>(apiUrl, action, apiLYRequest, "FundChecking");
        }

        public ValueTask<LYApiResponse<LYGetBalanceResponse>> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYGetBalanceRequest request)
        {
            var action = LYAction.GetBalance;
            request.Action = _utility.GetAction(action) ?? -1;

            var timestamp = DateTimeOffset.UtcNow.AddHours(+8).ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYGetBalanceResponse>(apiUrl, action, apiLYRequest, "Balance");
        }

        public ValueTask<LYApiResponse<LYGameUrlResponse>> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYGameUrlRequest request)
        {
            var action = LYAction.GetGameUrl;
            request.Action = _utility.GetAction(LYAction.GetGameUrl) ?? -1;

            var timestamp = dateTimeOffset.ToUnixTimeMilliseconds();
            var dataPath = _utility.GetPath(request);
            var param = _utility.AesEncrypt(dataPath, deskey, true);
            var plainText = $"{agentId}{timestamp}{md5key}";
            var key = _utility.HashMD5(plainText);

            var apiLYRequest = new LYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            return GetAPIAsync<LYGameUrlResponse>(apiUrl, action, apiLYRequest, "GameUrl");
        }

        private async ValueTask<LYApiResponse<TData>> GetAPIAsync<TData>(Uri apiUrl, LYAction action, LYApiRequest request, string logPrefix)
        {
            logPrefix = $"{nameof(LYApiClient).ToLower() }.{logPrefix}";
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(LYApiResponse<TData>);

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
                Prefix = $"{Consts.GameProvider.LY.ToLower()}.{action.ToString()}",
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
                response = JsonSerializer.Deserialize<LYApiResponse<TData>>(responseMessage);
            }

            return response;
        }
    }
}

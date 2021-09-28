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
        ValueTask<KYApiResponse<KYGetBalanceResponse>> GetBalanceAsync(KYBrandMerchant brandMerchant, string agentId, KYGetBalanceRequest request);
        ValueTask<KYApiResponse<KYGameUrlResponse>> GetGameUrlAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameUrlRequest request);
        ValueTask<KYApiResponse<KYGameRecordResponse>> GetGameRecordAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameRecordRequest request);
        ValueTask<KYApiResponse<KYKickResponse>> KickAsync(KYBrandMerchant brandMerchant, string agentId, KYKickRequest request);
        ValueTask<KYGPGameRecordResponse> GameRecordAsync(Uri apiUrl, KYGPGameRecordRequest request);
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

            return GetAPIAsync<KYGameRecordResponse>(brandMerchant.ApiRecordUrl, action, apiKYRequest);
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


        public ValueTask<KYGPGameRecordResponse> GameRecordAsync(Uri apiUrl, KYGPGameRecordRequest request) =>
            ExecuteJsonRequestAsync<Models.KY.Incoming.KYGPGameRecordRequest, KYGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/ky/GameRecord", request);

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
                ip = string.Empty,
                path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                prefix = $"{Consts.GameProvider.KY.ToLower()}.{action.ToString()}",
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
    }
}

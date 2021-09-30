using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tgs.Logging;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.GameProviders.Settings.TCG;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Incoming;
using TGS.Shared.Game.Models.TCG.Outgoing;

namespace TGS.Shared.Game.ApiClients.TCG
{
    public interface ITCGApiClient
    {
        ValueTask<TCGGPGameRecordResponse> GameRecordAsync(Uri apiUrl, TCGGPGameRecordRequest request);

        ValueTask<TCGKickOutLottoPlayerResponse> KickOutLottoPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGKickOutLottoPlayerRequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameByELOTTORequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameBySEARequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameByLOTTOVNRequest request);

        ValueTask<TCGRegisterPlayerResponse> RegisterPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGRegisterPlayerRequest request);

        ValueTask<TCGUpdatePasswordPlayerResponse> RegisterPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGUpdatePasswordPlayerRequest request);
    }

    public class TCGApiClient : BaseApiClient, ITCGApiClient
    {
        private readonly HttpClient _client;

        private readonly IJsonSerializationProvider _json;
        private readonly IApiClientLogger _logger;
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

        ValueTask<TCGGPGameRecordResponse> ITCGApiClient.GameRecordAsync(Uri apiUrl, TCGGPGameRecordRequest request) =>
         ExecuteJsonRequestAsync<TCGGPGameRecordRequest, TCGGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/tcg/GameRecord", request, logPrefix: _utility.GetPrefix("GameRecord"));

        ValueTask<TCGKickOutLottoPlayerResponse> ITCGApiClient.KickOutLottoPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGKickOutLottoPlayerRequest request)
       => GetAPIAsync<TCGKickOutLottoPlayerRequest, TCGKickOutLottoPlayerResponse>(ApiUrl, merchant, request, "KickOutLottoPlayer");

        ValueTask<TCGLaunchGameResponse> ITCGApiClient.LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameByELOTTORequest request)
         => GetAPIAsync<TCGLaunchGameByELOTTORequest, TCGLaunchGameResponse>(ApiUrl, merchant, request, "LaunchGameByELOTTO");

        ValueTask<TCGLaunchGameResponse> ITCGApiClient.LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameBySEARequest request)
         => GetAPIAsync<TCGLaunchGameBySEARequest, TCGLaunchGameResponse>(ApiUrl, merchant, request, "LaunchGameBySEA");

        ValueTask<TCGLaunchGameResponse> ITCGApiClient.LaunchGameAsync(Uri ApiUrl, TCGMerchant merchant, TCGLaunchGameByLOTTOVNRequest request)
         => GetAPIAsync<TCGLaunchGameByLOTTOVNRequest, TCGLaunchGameResponse>(ApiUrl, merchant, request, "LaunchGameByLOTTOVN");

        ValueTask<TCGRegisterPlayerResponse> ITCGApiClient.RegisterPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGRegisterPlayerRequest request)
                                                    => GetAPIAsync<TCGRegisterPlayerRequest, TCGRegisterPlayerResponse>(ApiUrl, merchant, request, "RegisterPlayer");

        ValueTask<TCGUpdatePasswordPlayerResponse> ITCGApiClient.RegisterPlayerAsync(Uri ApiUrl, TCGMerchant merchant, TCGUpdatePasswordPlayerRequest request)
            => GetAPIAsync<TCGUpdatePasswordPlayerRequest, TCGUpdatePasswordPlayerResponse>(ApiUrl, merchant, request, "RegisterPlayer");

        private async ValueTask<TResponse> GetAPIAsync<TRequest, TResponse>(Uri apiUrl, TCGMerchant merchant, TRequest request, string prefix) where TResponse : TCGBaseResponse
        {
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResponse);
            var jsonString = _json.SerializeToString(request);
            var encryptedParams = _utility.DESCEncrypt(jsonString, merchant.DESKey);
            var sign = _utility.GetHash256(encryptedParams, merchant.SHA256Key);
            var data =
                "merchant_code=" + WebUtility.UrlEncode(merchant.MerchantCode)
                + "&params=" + WebUtility.UrlEncode(encryptedParams)
                + "&sign=" + WebUtility.UrlEncode(sign);

            HttpContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

            var httpResponseMessage = await _client.PostAsync(apiUrl, content);
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
                Prefix = _utility.GetPrefix(prefix),
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = _json.SerializeToString(request),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = _json.DeserializeFromString<TResponse>(responseMessage);
            }

            return response;
        }
    }
}

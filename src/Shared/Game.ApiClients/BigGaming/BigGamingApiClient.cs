using System;
using System.Net.Http;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.BigGaming.Outgoing;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.Extensions;
using System.Text.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings.BigGaming;
using TGS.Shared.Game.Models.BigGaming.Commons;
using TGS.Shared.Game.Models.BigGaming.Incoming;
using System.Threading.Tasks;

namespace TGS.Shared.Game.ApiClients.BigGaming
{
    public interface IBigGamingApiClient
    {
        ValueTask<BigGamingResponse> CreateAgentAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreateAgentRequest request);
        ValueTask<BigGamingResponse> CreatePlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreatePlayerRequest request);
        ValueTask<BigGamingResponse> InfoPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingInfoPlayerRequest request);
        ValueTask<BigGamingResponse> KickPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingKickPlayerRequest request);
        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlRequest request);
        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlFishingRequest request);
        ValueTask<BigGamingResponse> GetTrialGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetTrialGameUrlRequest request);
        ValueTask<BigGamingResponse> GetVideoOrderDetailAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderDetailRequest request);
        ValueTask<BigGamingResponse> GetVideoRoundQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoRoundQueryRequest request);
        ValueTask<BigGamingResponse> GetVideoOrderQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderQueryRequest request);
        ValueTask<BigGamingResponse> GetGameRecordAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingRequest request);
        ValueTask<BigGamingGPGameRecordResponse> GameRecordAsync(Uri apiUrl, BigGamingGPGameRecordRequest request);
        ValueTask<BigGamingResponse> GetGameRecordFishingUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingUrlRequest request);
    }

    public sealed class BigGamingApiClient : BaseApiClient, IBigGamingApiClient
    {
        private readonly IHashProvider _hashProvider;
        private readonly string _gameProvider = Consts.GameProvider.BigGaming;

        public BigGamingApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json, IHashProvider hashProvider)
            : base(client, logger, json)
        {
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.CreateAgentAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreateAgentRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{request.LoginId}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.CreateAgent";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.agent.create", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.CreatePlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreatePlayerRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{BigGamingUtility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.CreatePlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.InfoPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingInfoPlayerRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{request.Sn}{request.LoginId}{brandMerchant.SecretCode}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.InfoPlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.get", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.KickPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingKickPlayerRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{request.Sn}{request.LoginId}{brandMerchant.SecretCode}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.KickPlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{request.LoginId}{BigGamingUtility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.GetGameUrl";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.game.url", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlFishingRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.GetGameUrlFish";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.game.bg.url", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetTrialGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetTrialGameUrlRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.GetTrialGameUrl";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.trial.game.url", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetVideoOrderDetailAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderDetailRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{request.Sn}{request.OrderId}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.VideoOrderDetail";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.sn.video.order.detail.url.get", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetVideoRoundQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoRoundQueryRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{request.Sn}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.VideoRoundQuery";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.round.query", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetVideoOrderQueryAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderQueryRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{request.Sn}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.VideoOrderQuery";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.order.query", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetGameRecordAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.GetGameRecordFishing";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.order.bg.query", prefix).ConfigureAwait(false);
        }

        async ValueTask<BigGamingResponse> IBigGamingApiClient.GetGameRecordFishingUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGameRecordFishingUrlRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{request.ExternalRoundId}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.GetGameRecordFishingUrl";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.sn.bg.order.detail.url.get", prefix).ConfigureAwait(false);
        }

        ValueTask<BigGamingResponse> GetAPIAsync<TSource>(Uri apiUrl, TSource source, string method, string prefix)
        {

            var request = new BigGamingRequest()
            {
                Id = GuidExtensions.NewSequentialGuid().ToString(),
                Method = method,
                Params = JsonDocument.Parse(JsonSerializer.Serialize(source)),
                Jsonrpc = "2.0"
            };
            return ExecuteJsonRequestAsync<BigGamingRequest, BigGamingResponse>(HttpMethod.Post, apiUrl, string.Empty, request, logPrefix: prefix);

        }

        ValueTask<BigGamingGPGameRecordResponse> IBigGamingApiClient.GameRecordAsync(Uri apiUrl, BigGamingGPGameRecordRequest request) =>
           ExecuteJsonRequestAsync<BigGamingGPGameRecordRequest, BigGamingGPGameRecordResponse>(HttpMethod.Post, apiUrl, "/api/bg/gamerecordfishing", request);
    }
}
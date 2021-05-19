using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Outgoing;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.Extensions;
using System.Text.Json;
using System.Collections.Generic;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings.BigGaming;

namespace TGS.Shared.Game.ApiClients.BigGaming
{
    public interface IBigGamingApiClient
    {
        Task<BigGamingBaseResponse> CreateAgentAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreateAgentRequest request);
        Task<BigGamingBaseResponse> CreatePlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreatePlayerRequest request);
        Task<BigGamingBaseResponse> InfoPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingInfoPlayerRequest request);
        Task<BigGamingBaseResponse> KickPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingKickPlayerRequest request);
        Task<BigGamingBaseResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlRequest request);
        Task<BigGamingBaseResponse> GetTrialGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetTrialGameUrlRequest request);
        Task<BigGamingBaseResponse> GetVideoOrderDetailAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderDetailRequest request);
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

        async Task<BigGamingBaseResponse> IBigGamingApiClient.CreateAgentAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreateAgentRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{request.LoginId}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.CreateAgent";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.agent.create", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.CreatePlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingCreatePlayerRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{BigGamingUtility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.CreatePlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.InfoPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingInfoPlayerRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{request.Sn}{request.LoginId}{brandMerchant.SecretCode}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.InfoPlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.get", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.KickPlayerAsync(BigGamingBrandMerchant brandMerchant, BigGamingKickPlayerRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{request.Sn}{request.LoginId}{brandMerchant.SecretCode}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.KickPlayer";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetGameUrlRequest request)
        {
            request.Digest = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{request.LoginId}{BigGamingUtility.GetSecretCode(brandMerchant.AgentLoginIdPassword)}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.GetGameUrl";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.game.url", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.GetTrialGameUrlAsync(BigGamingBrandMerchant brandMerchant, BigGamingGetTrialGameUrlRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.GetTrialGameUrl";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.video.trial.game.url", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.GetVideoOrderDetailAsync(BigGamingBrandMerchant brandMerchant, BigGamingVideoOrderDetailRequest request)
        {
            request.Sign = _hashProvider.Hash($"{request.Random }{request.Sn}{request.OrderId}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5);
            var prefix = $"{_gameProvider}.VideoOrderDetail";
            return await GetAPIAsync(brandMerchant.ApiUrl, request, "open.sn.video.order.detail.url.get", prefix).ConfigureAwait(false);
        }

        private ValueTask<BigGamingBaseResponse> GetAPIAsync<TSource>(Uri apiUrl, TSource source, string method, string prefix)
        {

            var request = new BigGamingBaseRequest()
            {
                Id = GuidExtensions.NewSequentialGuid().ToString(),
                Method = method,
                Params = JsonDocument.Parse(JsonSerializer.Serialize(source)),
                Jsonrpc = "2.0"
            };
            return ExecuteJsonRequestAsync<BigGamingBaseRequest, BigGamingBaseResponse>(HttpMethod.Post, apiUrl, string.Empty, request, logPrefix: prefix);

        }

    }
}
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

namespace TGS.Shared.Game.ApiClients.BigGaming
{
    public interface IBigGamingApiClient
    {
        Task<BigGamingBaseResponse> CreateAgentAsync(Uri apiUrl, BigGamingCreateAgentRequest request);
        Task<BigGamingBaseResponse> CreatePlayerAsync(Uri apiUrl, BigGamingCreatePlayerRequest request);
        Task<BigGamingInfoPlayerResponse> InfoPlayerAsync(Uri apiUrl, BigGamingInfoPlayerRequest request, string secretCode);
        Task<string> KickPlayerAsync(Uri apiUrl, BigGamingKickPlayerRequest request, string secretCode);
        Task<BigGamingBaseResponse> GetGameUrlAsync(Uri apiUrl, BigGamingGetGameUrlRequest request);
        Task<BigGamingBaseResponse> GetTrialGameUrlAsync(Uri apiUrl, BigGamingGetTrialGameUrlRequest request);
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

        async Task<BigGamingBaseResponse> IBigGamingApiClient.CreateAgentAsync(Uri apiUrl, BigGamingCreateAgentRequest request)
        {
            var prefix = $"{_gameProvider}.CreateAgent";
            return await GetAPIAsync(apiUrl, request, "open.agent.create", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.CreatePlayerAsync(Uri apiUrl, BigGamingCreatePlayerRequest request)
        {
            var prefix = $"{_gameProvider}.CreatePlayer";
            return await GetAPIAsync(apiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingInfoPlayerResponse> IBigGamingApiClient.InfoPlayerAsync(Uri apiUrl, BigGamingInfoPlayerRequest request, string secretCode)
        {
            var digest = _hashProvider.Hash($"{request.Random }{request.Sn}{request.LoginId}{secretCode}", HashAlgorithmKind.Md5);
            request.Digest = digest;
            var prefix = $"{_gameProvider}.CreatePlayer";
            var reslut = await GetAPIAsync(apiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
            if (reslut.Error == null)
            {
                return JsonSerializer.Deserialize<BigGamingInfoPlayerResponse>(JsonSerializer.Serialize(reslut.Result));
            }
            return null;
        }

        async Task<string> IBigGamingApiClient.KickPlayerAsync(Uri apiUrl, BigGamingKickPlayerRequest request, string secretCode)
        {
            var digest = _hashProvider.Hash($"{request.Random }{request.Sn}{request.LoginId}{secretCode}", HashAlgorithmKind.Md5);
            request.Digest = digest;
            var prefix = $"{_gameProvider}.KickPlayer";
            var reslut = await GetAPIAsync(apiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
            if (reslut.Error == null)
            {
                return JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(reslut.Result));
            }
            return null;
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.GetGameUrlAsync(Uri apiUrl, BigGamingGetGameUrlRequest request)
        {
            var prefix = $"{_gameProvider}.GetGameUrl";
            return await GetAPIAsync(apiUrl, request, "open.video.game.url", prefix).ConfigureAwait(false);
        }

        async Task<BigGamingBaseResponse> IBigGamingApiClient.GetTrialGameUrlAsync(Uri apiUrl, BigGamingGetTrialGameUrlRequest request)
        {
            var prefix = $"{_gameProvider}.GetTrialGameUrl";
            return await GetAPIAsync(apiUrl, request, "open.video.trial.game.url", prefix).ConfigureAwait(false);
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
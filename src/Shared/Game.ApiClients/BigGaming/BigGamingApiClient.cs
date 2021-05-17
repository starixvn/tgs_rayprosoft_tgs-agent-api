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
        Task<BGBaseResponse> CreateAgentAsync(Uri apiUrl, BGCreateAgentRequest request);
        Task<BGBaseResponse> CreatePlayerAsync(Uri apiUrl, BGCreatePlayerRequest request);
        Task<BGInfoPlayerResponse> InfoPlayerAsync(Uri apiUrl, BGInfoPlayerRequest request, string secretCode);
        Task<string> KickPlayerAsync(Uri apiUrl, BGKickPlayerRequest request, string secretCode);
        Task<BGBaseResponse> GetGameUrlAsync(Uri apiUrl, BGGetGameUrlRequest request);
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

        async Task<BGBaseResponse> IBigGamingApiClient.CreateAgentAsync(Uri apiUrl, BGCreateAgentRequest request)
        {
            var prefix = $"{_gameProvider}.CreateAgent";
            return await GetAPIAsync(apiUrl, request, "open.agent.create", prefix).ConfigureAwait(false);
        }

        async Task<BGBaseResponse> IBigGamingApiClient.CreatePlayerAsync(Uri apiUrl, BGCreatePlayerRequest request)
        {
            var prefix = $"{_gameProvider}.CreatePlayer";
            return await GetAPIAsync(apiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
        }

        async Task<BGInfoPlayerResponse> IBigGamingApiClient.InfoPlayerAsync(Uri apiUrl, BGInfoPlayerRequest request, string secretCode)
        {
            var digest = _hashProvider.Hash($"{request.Random }{request.Sn}{request.LoginId}{secretCode}", HashAlgorithmKind.Md5);
            request.Digest = digest;
            var prefix = $"{_gameProvider}.CreatePlayer";
            var reslut = await GetAPIAsync(apiUrl, request, "open.user.create", prefix).ConfigureAwait(false);
            if (reslut.Error == null)
            {
                return JsonSerializer.Deserialize<BGInfoPlayerResponse>(JsonSerializer.Serialize(reslut.Result));
            }
            return null;
        }

        async Task<string> IBigGamingApiClient.KickPlayerAsync(Uri apiUrl, BGKickPlayerRequest request, string secretCode)
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

        async Task<BGBaseResponse> IBigGamingApiClient.GetGameUrlAsync(Uri apiUrl, BGGetGameUrlRequest request)
        {
            var prefix = $"{_gameProvider}.GameUrl";
            return await GetAPIAsync(apiUrl, request, "open.video.game.url", prefix).ConfigureAwait(false);
        }

        private ValueTask<BGBaseResponse> GetAPIAsync<TSource>(Uri apiUrl, TSource source, string method, string prefix)
        {

            var request = new BGBaseRequest()
            {
                Id = GuidExtensions.NewSequentialGuid().ToString(),
                Method = method,
                Params = JsonDocument.Parse(JsonSerializer.Serialize(source)),
                Jsonrpc = "2.0"
            };
            return ExecuteJsonRequestAsync<BGBaseRequest, BGBaseResponse>(HttpMethod.Post, apiUrl, string.Empty, request, logPrefix: prefix);

        }

    }
}
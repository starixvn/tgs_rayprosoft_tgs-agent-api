using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Shared.Game.Models.Card365.Outgoing;

namespace TGS.Shared.Game.ApiClients.Card365
{
    public interface ICard365ApiClient
    {
        ValueTask<GetKeyResponse> GetAccessKeyAsync(Uri apiUrl, string website, GetKeyRequest request);

        ValueTask<LogoutResponse> LogoutAsync(Uri apiUrl, string website, LogoutRequest request);
    }

    public class Card365ApiClient : BaseApiClient, ICard365ApiClient
    {
        public Card365ApiClient(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        public ValueTask<GetKeyResponse> GetAccessKeyAsync(Uri apiUrl, string website, GetKeyRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.Card365}.GetKey";
            return ExecuteFormRequestAsync<GetKeyResponse>(apiUrl, $"{website}/getKey",
                parameters, logPrefix: prefix);
        }

        public ValueTask<LogoutResponse> LogoutAsync(Uri apiUrl, string website, LogoutRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.Card365}.Logout";
            return ExecuteFormRequestAsync<LogoutResponse>(apiUrl, $"{website}/logout",
                parameters, logPrefix: prefix);
        }
    }
}
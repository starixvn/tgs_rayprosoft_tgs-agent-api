using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Shared.Game.Models.WMCasino.Outgoing;

namespace TGS.Shared.Game.ApiClients.WMCasino
{
    public interface IWMCasinoApiClient
    {
        ValueTask<LoginGameResponse> LoginGameAsync(Uri apiUrl, LoginGameRequest request);
        ValueTask<MemberRegisterResponse> MemberRegisterAsync(Uri apiUrl, MemberRegisterRequest request);
    }

    public class WMCasinoApiClient : BaseApiClient, IWMCasinoApiClient
    {
        public WMCasinoApiClient(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        public ValueTask<LoginGameResponse> LoginGameAsync(Uri apiUrl, LoginGameRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.WMCasino}.LoginGame";
            return ExecuteFormRequestAsync<LoginGameResponse>(apiUrl, string.Empty,
                parameters, logPrefix: prefix);
        }

        public ValueTask<MemberRegisterResponse> MemberRegisterAsync(Uri apiUrl, MemberRegisterRequest request)
        {
            var value = Json.SerializeToString(request);
            var parameters = Json.DeserializeFromString<Dictionary<string, string>>(value);

            var prefix = $"{Consts.GameProvider.WMCasino}.MemberRegister";
            return ExecuteFormRequestAsync<MemberRegisterResponse>(apiUrl, string.Empty,
                parameters, logPrefix: prefix);
        }
    }
}
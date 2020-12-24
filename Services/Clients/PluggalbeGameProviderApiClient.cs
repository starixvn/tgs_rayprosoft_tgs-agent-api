using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages.Pluggables;

namespace TGS.Services.Clients
{
    public interface IPluggalbeGameProviderApiClient
    {
        Task<GetGameUrlResponse> GetGameUrlAsync(Uri apiUrl, GetGameUrlRequest request);
    }

    public sealed class PluggalbeGameProviderApiClient : BaseApiClient, IPluggalbeGameProviderApiClient
    {
        public PluggalbeGameProviderApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) 
            : base(client, logger, json)
        {
        }

        Task<GetGameUrlResponse> IPluggalbeGameProviderApiClient.GetGameUrlAsync(Uri apiUrl, GetGameUrlRequest request)
        {
            var prefix = $"Pluggable.{request.GameProviderCode}.GetGameUrl";
            return ExecuteJsonRequestAsync<GetGameUrlRequest, GetGameUrlResponse>(HttpMethod.Post, apiUrl,
                "/api/pluggable/gameurl", request, logPrefix: prefix);
        }
    }
}

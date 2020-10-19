// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Net.Http;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface ISBOBETApiClient
    {
        Task<GetBalanceResponse> GetPlayerBalanceAsync(GetBalanceRequest request);
    }

    public sealed class SBOBETApiClient :  BaseApiClient, ISBOBETApiClient
    {
        public SBOBETApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) 
            : base(client, logger, json)
        {
        }

        Task<GetBalanceResponse> ISBOBETApiClient.GetPlayerBalanceAsync(GetBalanceRequest request) =>
            ExecuteJsonRequestAsync<GetBalanceRequest, GetBalanceResponse>(HttpMethod.Post, "api/GetBalance", request);
    }
}

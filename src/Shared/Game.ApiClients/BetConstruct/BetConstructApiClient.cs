using System.Net.Http;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;

namespace TGS.Shared.Game.ApiClients.BetConstruct;

public interface IBetConstructApiClient
{
}

public sealed class BetConstructApiClient : BaseApiClient, IBetConstructApiClient
{
    public BetConstructApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
        : base(client, logger, json)
    {
    }
}

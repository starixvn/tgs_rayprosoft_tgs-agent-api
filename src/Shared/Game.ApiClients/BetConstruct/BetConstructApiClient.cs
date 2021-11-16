using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;

namespace TGS.Shared.Game.ApiClients.BetConstruct
{
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
}

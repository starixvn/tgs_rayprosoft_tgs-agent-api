using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.NotifyApi.Messages;

namespace TGS.Apis.LogApi.Clients
{
    public interface INotifyClient
    {
        ValueTask<NotifyResponse> SendAsync(NotifyLogRequest request);
    }
    public class NotifyClient : BaseApiClient, INotifyClient
    {
        private readonly IConfiguration _configuration;
        public NotifyClient(IConfiguration configuration, 
            HttpClient client, 
            IApiClientLogger logger, 
            IJsonSerializationProvider json)
          : base(client, logger, json)
        {
            _configuration = configuration;
        }

        ValueTask<NotifyResponse> INotifyClient.SendAsync(NotifyLogRequest request)
        {
            var path = _configuration.GetSection("Notify:Path");
            return ExecuteJsonRequestAsync<NotifyLogRequest, NotifyResponse > (HttpMethod.Post, path.Value, request);
        }
    }
}

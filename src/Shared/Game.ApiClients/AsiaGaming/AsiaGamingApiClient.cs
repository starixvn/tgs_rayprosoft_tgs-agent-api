using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Shared.Game.Models.AsiaGaming.Outgoing;

namespace TGS.Shared.Game.ApiClients.AsiaGaming
{
    public interface IAsiaGamingApiClient
    {
        Task<CheckOrCreateAccountResponse> CheckOrCreateAccountAsync(Uri apiUrl, string parameter, string key);

        Task<PlayerTicketResponse> CreateGameSessionAsync(Uri apiUrl, string queryString);
    }

    public sealed class AsiaGamingApiClient : BaseApiClient, IAsiaGamingApiClient
    {
        private static readonly XmlMediaTypeFormatter XmlMediaTypeFormatter = new();

        public AsiaGamingApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        Task<CheckOrCreateAccountResponse> IAsiaGamingApiClient.CheckOrCreateAccountAsync(Uri apiUrl, string parameter, string key)
        {
            var logPrefix = $"{Consts.GameProvider.AsiaGaming}.CheckOrCreateAccount";
            var queryString = $"params={parameter}&key={key}";
            return GetDeserializedDataAsync<CheckOrCreateAccountResponse>(apiUrl, $"doBusiness.do?{queryString}", logPrefix);
        }

        Task<PlayerTicketResponse> IAsiaGamingApiClient.CreateGameSessionAsync(Uri apiUrl, string queryString)
        {
            var logPrefix = $"{Consts.GameProvider.AsiaGaming}.CreateGameSession";
            return GetDeserializedDataAsync<PlayerTicketResponse>(apiUrl, $"resource/player-tickets.ucs?{queryString}", logPrefix);
        }

        private static T DeserializeWithXmlSerializer<T>(string msg)
        {
            using var sr = new StringReader(msg);
            var serializer = XmlMediaTypeFormatter.CreateXmlSerializer(typeof(T));
            return (T)serializer.Deserialize(sr);
        }

        private async Task<T> GetDeserializedDataAsync<T>(Uri apiUrl, string path, string logPrefix)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl.AbsoluteUri.TrimEndSlash()}/{path}");
            var resp = await SendAsync(req, logPrefix);
            var xml = await resp.Content.ReadAsStringAsync();
            return DeserializeWithXmlSerializer<T>(xml);
        }
    }
}
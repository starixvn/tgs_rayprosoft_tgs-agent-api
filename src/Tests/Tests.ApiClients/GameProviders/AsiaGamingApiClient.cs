using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IAsiaGamingApiClient
    {
        Task<TransferResponse> PostTransferAsync(PostTransferRequest request);
        Task<TransferResponse> BetResponseAsync(BetResponseRequest request);
    }

    public sealed class AsiaGamingApiClient : BaseApiClient, IAsiaGamingApiClient
    {
        private static readonly string RoutePrefix = "api/ag";
        private readonly HttpClient _client;

        public AsiaGamingApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json) :
            base(client, logger, json)
        {
            _client = client;
        }

        Task<TransferResponse> IAsiaGamingApiClient.PostTransferAsync(PostTransferRequest request) =>
            ExecuteXmlRequestAsync<PostTransferRequest, TransferResponse>(HttpMethod.Post,
                $"{RoutePrefix}/posttransfer", request);

        Task<TransferResponse> IAsiaGamingApiClient.BetResponseAsync(BetResponseRequest request) =>
            ExecuteXmlRequestAsync<BetResponseRequest, TransferResponse>(HttpMethod.Post,
                $"{RoutePrefix}/betresponse", request);

        private async Task<TResponse> ExecuteXmlRequestAsync<TRequest, TResponse>(HttpMethod method, string path,
            TRequest request)
        {
            var requestMessage = CreateHttpRequestMessage(path, method);
            var httpContent = new StringContent(ObjectToXml(request), Encoding.UTF8, MediaTypeNames.Text.Xml);
            requestMessage.Content = httpContent;

            var response = await SendAsync(requestMessage, string.Empty);

            var responseString = await response.Content.ReadAsStringAsync();
            return DeserializeXml<TResponse>(responseString);
        }

        private HttpRequestMessage CreateHttpRequestMessage(string path, HttpMethod method)
        {
            var reqUrl = _client.BaseAddress!.AbsoluteUri.TrimEndSlash();
            reqUrl = $"{reqUrl}/{path}";
            return new(method, reqUrl);
        }

        private static string ObjectToXml<TRequest>(TRequest request)
        {
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add("", "");
            using var sw = new Utf8StringWriter();
            using var writer = XmlWriter.Create(sw);
            var serializer = new XmlSerializer(typeof(TRequest));
            serializer.Serialize(writer, request, emptyNamespaces);
            return sw.ToString();
        }

        private static T DeserializeXml<T>(string msg)
        {
            using var sr = new StringReader(msg);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(sr);
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => new UTF8Encoding(false);
        }
    }
}
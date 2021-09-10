using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log.Events;
using TGS.GameProviders.Repositories.Options;

namespace TGS.GameProviders.Repositories
{
    internal sealed class GameProviderTCGFtpClientRepository : IGameProviderTCGFtpClientRepository
    {
        private readonly IApiClientLogger _logger;
        private readonly IJsonSerializationProvider _json;
        private readonly HttpClient _client;
        public GameProviderTCGFtpClientRepository(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _json = json ?? throw new ArgumentNullException(nameof(json));
        }

        public async Task<string> GetSettledFileContentAsync(string path, string merchant, string directory, string file)
        {
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var parameters = new Dictionary<string, string>()
            {
                { "merchant",merchant},
                { "directory",directory},
                { "file",file}
            };
            var paths = $"{path}/GetSettledFileContent?{string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))}";
            var httpResponseMessage = await _client.GetAsync(paths).ConfigureAwait(false);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _logger.LogHttpResponse(new ApiLogEvent
            {
                ip = string.Empty,
                path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                prefix = "TCGFtpClient",
                servername = Environment.MachineName.NoLongerThan(128),
                requestheaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                requestcontent = null,
                responseheaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                responsecontent = responseMessage,
                responsecode = (int)httpResponseMessage.StatusCode,
                executiontimems = (int)sw.ElapsedMilliseconds,
                recordedon = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                return responseMessage;
            }
            return null;
        }

        public async Task<string[]> GetSettledFileListAsync(string path, string merchant, string directory)
        {
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var parameters = new Dictionary<string, string>()
            {
                { "merchant",merchant},
                { "directory",directory},
            };
            var paths = $"{path}/GetSettledFileList?{string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))}";
            var httpResponseMessage = await _client.GetAsync(paths).ConfigureAwait(false);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _logger.LogHttpResponse(new ApiLogEvent
            {
                ip = string.Empty,
                path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                prefix = "TCGFtpClient",
                servername = Environment.MachineName.NoLongerThan(128),
                requestheaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                requestcontent = null,
                responseheaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                responsecontent = responseMessage,
                responsecode = (int)httpResponseMessage.StatusCode,
                executiontimems = (int)sw.ElapsedMilliseconds,
                recordedon = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                return _json.DeserializeFromString<string[]>(responseMessage);
            }
            return null;
        }
    }
}

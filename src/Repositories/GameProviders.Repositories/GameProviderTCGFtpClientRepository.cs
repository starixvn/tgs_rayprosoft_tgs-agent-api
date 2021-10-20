using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tgs.Logging;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient;

namespace TGS.GameProviders.Repositories
{
    internal sealed class GameProviderTCGFtpClientRepository : IGameProviderTCGFtpClientRepository
    {
        private readonly HttpClient _client;
        private readonly IJsonSerializationProvider _json;
        private readonly IApiClientLogger _logger;

        public GameProviderTCGFtpClientRepository(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider json)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _json = json ?? throw new ArgumentNullException(nameof(json));
        }

        public async Task<GetSettledFileContentResponse> GetSettledFileContentAsync(string path, string merchant, string directory, string file)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "merchant",merchant},
                { "directory",directory},
                { "file",file}
            };
            var paths = $"{path}/GetSettledFileContent?{string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))}";

            string result = await FtpClientGetAsync(paths, "GetSettledFileContent").ConfigureAwait(false);

            if (!string.IsNullOrEmpty(result))
            {
                return _json.DeserializeFromString<GetSettledFileContentResponse>(result);
            }

            return null;
        }

        public async Task<string[]> GetSettledFileListAsync(string path, string merchant, string directory)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "merchant",merchant},
                { "directory",directory},
            };
            var paths = $"{path}/GetSettledFileList?{string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)))}";

            string result = await FtpClientGetAsync(paths, "GetSettledFileList").ConfigureAwait(false);

            if (!string.IsNullOrEmpty(result))
            {
                return _json.DeserializeFromString<string[]>(result);
            }

            return null;
        }

        private async Task<string> FtpClientGetAsync(string paths, string prefix)
        {
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var httpResponseMessage = default(HttpResponseMessage);
            var logPrefix = $"tcg.ftp.client.{prefix}";
            try
            {
                httpResponseMessage = await _client.GetAsync(paths).ConfigureAwait(false);
                sw.Stop();

                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
                    {
                        responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
                    }
                }
                _logger.LogHttpResponse(new ApiLogEvent
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = logPrefix,
                    ServerName = Environment.MachineName.NoLongerThan(128),
                    RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                    RequestContent = null,
                    ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                    ResponseContent = responseMessage,
                    ResponseCode = (short)httpResponseMessage.StatusCode,
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    RecordedOn = DateTimeOffset.UtcNow
                });
            }
            catch (Exception ex)
            {
                sw.Stop();
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(
                      $"Connect to remote host timeout {Environment.NewLine}{Environment.NewLine}{ex}"),
                    RequestMessage = null
                };

                await _logger.LogHttpResponseAsync(response, (int)sw.ElapsedMilliseconds, null, logPrefix);
            }

            return responseMessage;
        }
    }
}

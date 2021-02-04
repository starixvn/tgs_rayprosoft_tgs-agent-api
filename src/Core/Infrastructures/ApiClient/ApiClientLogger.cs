using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Log.Events;

namespace TGS.Core.Infrastructures.ApiClient
{
    public interface IApiClientLogger
    {
        Task LogHttpResponseAsync(HttpResponseMessage response, int executionTimeMs, string requestContent, string logPrefix);
    }

    public sealed class ApiClientLogger : IApiClientLogger
    {
        private readonly ITgsLog _log;

        public ApiClientLogger(ITgsLog log)
        {
            _log = log;
        }

        async Task IApiClientLogger.LogHttpResponseAsync(HttpResponseMessage response, int executionTimeMs, string requestContent, string logPrefix)
        {
            var request = response.RequestMessage;
            var resMessage = await response.Content?.ReadAsStringAsync();

            var apiCallLogEvent =
                new ApiLogEvent
                {
                    ip = string.Empty,
                    path = request.RequestUri.AbsoluteUri.NoLongerThan(512),
                    method = request.Method.ToString().NoLongerThan(8),
                    prefix = logPrefix.IsNullOrEmpty() ? GetPrefix(request.RequestUri.AbsolutePath) : logPrefix,
                    servername = Environment.MachineName.NoLongerThan(128),
                    requestheaders = request.Headers + request.Content?.Headers?.ToString(),
                    requestcontent = requestContent,
                    responseheaders = response.Headers.ToString(),
                    responsecontent = resMessage,
                    responsecode = (int)response.StatusCode,
                    executiontimems = executionTimeMs,
                    recordedon = DateTimeOffset.UtcNow
                };

            _log.LogApi(apiCallLogEvent);
        }

        private static string GetPrefix(string path)
        {
            var arr = path.Split('/');
            return arr.Length < 3 ? arr[^1] : arr[2];
        }
    }
}

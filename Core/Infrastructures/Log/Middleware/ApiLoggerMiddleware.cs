using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IO;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log.Attributes;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Core.Infrastructures.Log.Middleware
{
    public sealed class ApiLoggerMiddleware
    {
        private const int ResMessageSizeLimit = 1000012;

        private static readonly string[] BypassRouting = {"/", "/favicon.ico", "/health"};
        private readonly RequestDelegate _next;

        private readonly string _prefix;

        //avoid LOH :https://stackoverflow.com/questions/43403941/how-to-read-asp-net-core-response-body/52328142#52328142
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public ApiLoggerMiddleware(RequestDelegate next, string prefix)
        {
            _prefix = prefix;
            _next = next;

            _recyclableMemoryStreamManager = new();
        }

        private async Task<(string resMessage, int elapsedMilliseconds)> GetResponseMessageAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseStream;

            try
            {
                var sw = Stopwatch.StartNew();
                await _next(context);
                sw.Stop();
                var ms = (int) sw.ElapsedMilliseconds;
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                using var sr = new StreamReader(context.Response.Body);
                var message = await sr.ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await context.Response.Body.CopyToAsync(originalBodyStream);
                return (message, ms);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            requestStream.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(requestStream);
            var message = await sr.ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            return message;
        }

        private static string CutOffResponseContent(string responseContent) =>
            $"{responseContent.NoLongerThan(ResMessageSizeLimit)} ... Response content is too long. Length: {responseContent.Length}";

        public async Task InvokeAsync(HttpContext context, IRequestAnalyzer requestAnalyzer, ITgsLog log)
        {
            var path = context.Request.Path.Value;
            if (BypassRouting.Any(r => path.IgnoreCaseEquals(r)))
            {
                await _next(context);
                return;
            }

            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata.GetMetadata<SuppressApiLogAttribute>() != null)
            {
                await _next(context);
                return;
            }

            log.TrackingId = GuidExtensions.NewSequentialGuid();

            var requestTime = DateTimeOffset.UtcNow;

            var reqMessage = await GetRequestBodyAsync(context);

            var (resMessage, ms) = await GetResponseMessageAsync(context);

            var url = context.Request.GetEncodedUrl();
            var apiLogEvent =
                new ApiLogEvent
                {
                    ip = requestAnalyzer.GetIp(context).NoLongerThan(256),
                    path = url.NoLongerThan(256),
                    method = context.Request.Method.NoLongerThan(8),
                    prefix = GetPrefix(path),
                    servername = Environment.MachineName.NoLongerThan(128),
                    requestheaders = context.Request.Headers.ToDetailString(),
                    requestcontent = reqMessage,
                    responseheaders = context.Response.Headers.ToDetailString(),
                    responsecontent = resMessage?.Length > ResMessageSizeLimit
                        ? CutOffResponseContent(resMessage)
                        : resMessage,
                    responsecode = context.Response.StatusCode,
                    executiontimems = ms,
                    recordedon = requestTime
                };

            log.LogApi(apiLogEvent);
        }

        private string GetPrefix(string path)
        {
            if (_prefix.IsNotNullOrEmpty())
            {
                return _prefix;
            }

            var arr = path.Split('/');
            return arr.Length < 3 ? arr[^1] : arr[2];
        }
    }
}
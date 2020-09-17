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
using TGS.Core.Infrastructures.Log.Services;

namespace TGS.Core.Infrastructures.Log.Middleware
{
    public sealed class ApiLoggerMiddleware : IMiddleware
    {
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly ITgsLog _tgsLog;
        private readonly Func<string> _prefixFunc;

        private const int ResMessageSizeLimit = 1000012;

        private static readonly string[] BypassRouting = { "/", "/favicon.ico" };

        //avoid LOH :https://stackoverflow.com/questions/43403941/how-to-read-asp-net-core-response-body/52328142#52328142
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public ApiLoggerMiddleware(
            IRequestAnalyzer requestAnalyzer,
            ITgsLog tgsLog,
            Func<string> prefixFunc)
        {
            _requestAnalyzer = requestAnalyzer;
            _tgsLog = tgsLog;
            _prefixFunc = prefixFunc;

            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        private async Task<(string resMessage, int elapsedMilliseconds)> GetResponseMessageAsync(HttpContext context,
            RequestDelegate next)
        {
            var originalBodyStream = context.Response.Body;
            var responseStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseStream;

            try
            {
                var sw = Stopwatch.StartNew();
                await next(context);
                sw.Stop();
                var ms = (int)sw.ElapsedMilliseconds;
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
                responseStream.Dispose();
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

        private static string CutOffResponseContent(string responseContent)
        {
            return
                $"{responseContent.NoLongerThan(ResMessageSizeLimit)} ... Response content is too long. Length: {responseContent.Length}";
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestPath = context.Request.Path.Value;
            if (BypassRouting.Any(r => requestPath.IgnoreCaseEquals(r)))
            {
                await next(context);
                return;
            }

            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata.GetMetadata<SuppressApiLogAttribute>() != null)
            {
                await next(context);
                return;
            }

            _tgsLog.TrackingId = GuidExtensions.NewSequentialGuid();

            var requestTime = DateTimeOffset.UtcNow;

            var reqMessage = await GetRequestBodyAsync(context);

            var (resMessage, ms) = await GetResponseMessageAsync(context, next);

            var path = context.Request.GetEncodedUrl();
            var apiLogEvent =
                new ApiLogEvent
                {
                    ip = _requestAnalyzer.GetIp(context).NoLongerThan(256),
                    path = path.NoLongerThan(512),
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

            _tgsLog.LogApi(apiLogEvent);
        }

        private string GetPrefix(string path)
        {
            var prefix = default(string);

            if (_prefixFunc != null)
                prefix = _prefixFunc();

            if (prefix.IsNotNullOrEmpty())
                return prefix;

            var arr = path.Split('/');
            return arr.Length < 3 ? arr[^1] : arr[2];
        }
    }
}
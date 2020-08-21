using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IO;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Log.Attributes;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Log.Services;

namespace TGS.Infrastructures.Log.Middleware
{
    public sealed class ApiLoggerMiddleware : IMiddleware
    {
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly ITgsLog _logger;

        private const int ResMessageSizeLimit = 1000012;

        private static readonly string[] BypassRouting = { "/", "/favicon.ico" };

        //avoid LOH :https://stackoverflow.com/questions/43403941/how-to-read-asp-net-core-response-body/52328142#52328142
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        //[Dependency("Prefix")] public Func<string> PrefixFunc { get; set; } //Resolved by  InjectionProperty

        public ApiLoggerMiddleware(
            IRequestAnalyzer requestAnalyzer,
            ITgsLog logger)
        {
            _requestAnalyzer = requestAnalyzer;
            _logger = logger;

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

        private static string GetPrefix(string path)
        {
            var arr = path.Split('/');
            return arr.Length < 3 ? arr[arr.Length - 1] : arr[2];
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

            _logger.TrackingId = GuidExtensions.NewSequentialGuid();

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
                    //prefix = PrefixFunc == null || PrefixFunc().IsNullOrEmpty()
                    //    ? GetPrefix(path).NoLongerThan(128)
                    //    : PrefixFunc(),
                    prefix = path = path.NoLongerThan(512),
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

            _logger.LogApi(apiLogEvent);
        }
    }
}

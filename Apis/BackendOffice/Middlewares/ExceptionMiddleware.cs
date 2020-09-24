using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Infrastructures.Log.Services;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private const int ResMessageSizeLimit = 1000012;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly ITgsLog _tgsLog;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public ExceptionMiddleware(IRequestAnalyzer requestAnalyzer, ITgsLog logger)
        {
            _requestAnalyzer = requestAnalyzer;
            _tgsLog = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        async Task IMiddleware.InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            var adminLogEvent = new AdminLogEvent();

            try
            {
                var reqMessage = await GetRequestBodyAsync(httpContext);

                adminLogEvent.ipaddress = _requestAnalyzer.GetIp(httpContext).NoLongerThan(256);
                adminLogEvent.path = httpContext.Request.Path.Value;
                adminLogEvent.method = httpContext.Request.Method.NoLongerThan(8);
                adminLogEvent.host = httpContext.Request.Host.Value;
                adminLogEvent.servername = Environment.MachineName.NoLongerThan(128);
                adminLogEvent.requestheaders = httpContext.Request.Headers.ToDetailString();
                adminLogEvent.requestcontent = reqMessage;
                adminLogEvent.responseheaders = httpContext.Response.Headers.ToDetailString();

                adminLogEvent.recordedon = DateTimeOffset.Now;
                adminLogEvent.StackTrace = string.Empty;
                adminLogEvent.exception = string.Empty;

                var (resMessage, ms) = await GetResponseMessageAsync(httpContext, next);

                var descriptionAttribute = httpContext.GetEndpoint()?.Metadata.GetMetadata<DescriptionAttribute>();

                if (descriptionAttribute != null)
                {
                    adminLogEvent.description = descriptionAttribute.Description;
                }

                adminLogEvent.responsecontent = resMessage?.Length > ResMessageSizeLimit
                    ? CutOffResponseContent(resMessage)
                    : resMessage;

                adminLogEvent.responsecode = httpContext.Response.StatusCode;
                adminLogEvent.executiontimems = ms;
            }
            catch (Exception ex)
            {
                adminLogEvent.exception = ex.Message.NoLongerThan(4000);
                adminLogEvent.StackTrace = ex.StackTrace.NoLongerThan(4000);
                adminLogEvent.responsecode = (int)HttpStatusCode.InternalServerError;

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var message = new ErrorDetails
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Internal Server Error."
                };

                IJsonSerializationProvider json = new JsonSerializationProvider();
                var result = json.SerializeToString(message);

                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(result));
            }

            _tgsLog.LogAdmin(adminLogEvent);
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
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.Commons.Middlewares
{
    public abstract class BaseHandleGameProviderExceptionMiddleware : IMiddleware
    {
        protected string LoggerName { get; }
        protected IErrorMapper ErrorMapper { get; }
        protected ITgsLog Log { get; }

        protected BaseHandleGameProviderExceptionMiddleware(Func<string, IErrorMapper> mapper, string mapperKey, ITgsLog log, string loggerName)
        {
            LoggerName = loggerName;

            ErrorMapper = mapper(mapperKey);
            Log = log.SetLogger(LoggerName);
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                var errorCode = ErrorMapper.GetApiErrorByException(exception);
                var statusCode = ErrorMapper.GetHttpStatusByException(exception);

                var message = await GetErrorMessage(context, $"{LoggerName} Api Error", errorCode.ToString(), statusCode);
                Log.LogError(message, exception);

                await CreateResponseAsync(context, statusCode, errorCode, exception);
            }
        }

        protected abstract Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode, Exception exception);

        private static async Task<string> GetErrorMessage(HttpContext context, string type, string errorMessage, int statusCode)
        {
            string body;

            try
            {
                body = await context.Request.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                body = e.Message;
            }

            return new
            {
                type,
                error = errorMessage,
                statusCode,
                requestHeaders = GetRequestHeadersAsString(context.Request),
                requestMethod = context.Request.Method,
                requestUrl = context.Request.Path,
                request = body
            }
            .ToJson();
        }

        private static string GetRequestHeadersAsString(HttpRequest request)
        {
            return request.Headers
                .Aggregate(new StringBuilder(), (b, p) => b.AppendLine($"{p.Key}={p.Value.JoinAsStringBySemicolon()}"))
                .ToString();
        }
    }
}

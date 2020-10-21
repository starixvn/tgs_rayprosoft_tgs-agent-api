using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.Filters
{
    public abstract class BaseHandleErrorFilter : IAsyncActionFilter
    {
        protected abstract string LoggerName { get; }

        protected IErrorMapper ErrorMapper { get; }
        protected ITgsLog Log { get; }

        private IDictionary<string, object> _actionArguments;

        public BaseHandleErrorFilter(Func<string, IErrorMapper> mapper, string mapperKey, ITgsLog log)
        {
            ErrorMapper = mapper(mapperKey);
            Log = log.SetLogger(LoggerName);
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _actionArguments = context.ActionArguments;

            var aec = await next();

            var exception = aec.Exception;
            if (exception == null || aec.ExceptionHandled)
            {
                return;
            }

            aec.ExceptionHandled = true;

            var error = ErrorMapper.GetApiErrorByException(exception);
            var statusCode = ErrorMapper.GetHttpStatusByException(exception);

            var message = GetErrorMessage(aec, "Tgs Api Error", error.ToString(), statusCode);
            Log.LogError(message, aec.Exception);

            aec.Result = await CreateResponseAsync(aec, statusCode, error);
        }

        protected abstract Task<IActionResult> CreateResponseAsync(ActionExecutedContext context, int statusCode, int error);

        #region Private Methods

        private string GetErrorMessage(ActionExecutedContext context, string type, string errorMessage, int statusCode)
        {
            var reqParams = new List<object>();

            try
            {
                foreach (var arg in _actionArguments)
                {
                    reqParams.Add(new { key = arg.Key, parameter = arg.Value });
                }
            }
            catch (Exception ex)
            {
                reqParams.Add(ex.Message);
            }

            return new
            {
                type,
                error = errorMessage,
                statusCode,
                requestHeaders = GetRequestHeadersAsString(context.HttpContext.Request),
                requestMethod = context.HttpContext.Request.Method,
                requestUrl = context.HttpContext.Request.Path,
                requestParams = reqParams
            }
            .ToJson();
        }

        private static string GetRequestHeadersAsString(HttpRequest request)
        {
            return request.Headers
                .Aggregate(new StringBuilder(), (b, p) => b.AppendLine($"{p.Key}={p.Value.JoinAsStringBySemicolon()}"))
                .ToString();
        }

        #endregion Private Methods
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.Filters
{
    public abstract class BaseHandleErrorFilter : IActionFilter
    {
        protected abstract string LoggerName { get; }

        protected IErrorMapper ErrorMapper { get; }
        protected ITgsLog Log { get; }

        private IDictionary<string, object> _actionArguments;

        public BaseHandleErrorFilter(Func<string, IErrorMapper> mapper, string mapperKey,  ITgsLog log)
        {
            ErrorMapper = mapper(mapperKey);
            Log = log.SetLogger(LoggerName);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _actionArguments = context.ActionArguments;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var exception = context.Exception;
            if (exception == null || context.ExceptionHandled)
                return;

            context.ExceptionHandled = true;

            var error = ErrorMapper.GetApiErrorByException(exception);
            var statusCode = ErrorMapper.GetHttpStatusByException(exception);

            var message = GetErrorMessage(context, "Tgs Api Error", error.ToString(), statusCode);
            Log.LogError(message, context.Exception);

            context.Result = CreateResponse(context, statusCode, error);
        }

        protected abstract IActionResult CreateResponse(ActionExecutedContext context, int statusCode, int error);

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

        private string GetRequestHeadersAsString(HttpRequest request)
        {
            return request.Headers
                .Aggregate(new StringBuilder(), (b, p) => b.AppendLine($"{p.Key}={p.Value.JoinAsStringBySemicolon()}"))
                .ToString();
        }

        #endregion Private Methods
    }
}
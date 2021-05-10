using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Middlewares
{
    public class OperatorApiErrorMapperMiddleware
    {
        private readonly OperatorApiErrorMapper _errorMapper;
        private readonly RequestDelegate _next;

        public OperatorApiErrorMapperMiddleware(RequestDelegate next, OperatorApiErrorMapper errorMapper)
        {
            _next = next;
            _errorMapper = errorMapper;
        }

        public async Task InvokeAsync(HttpContext context, ITgsLog log)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                if (!_errorMapper.CanProcess(exception))
                {
                    throw;
                }

                var error = _errorMapper.GetApiErrorByException(exception);
                var statusCode = _errorMapper.GetHttpStatusByException(exception);
                var message = exception.GetMessageChain();

                log.LogError($"Caught global error on :{context.Request.Host} with message: {message}", exception);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(new TgsResponse {Error = error, ErrorMessage = message});
            }
        }
    }
}
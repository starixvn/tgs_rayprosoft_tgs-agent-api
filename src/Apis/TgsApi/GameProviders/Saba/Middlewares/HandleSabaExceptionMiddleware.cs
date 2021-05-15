using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Saba.Enums;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Middlewares
{
    public sealed class HandleSabaExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleSabaExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, Consts.GameProvider.Saba, log, "saba")
        {
        }

        protected override Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);
            var desc = error.Description(message);

            var res = new ErrorResponse
            {
                Status = errorCode.ToString(),
                Message = desc
            };

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(res);
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Card365.Enums;
using TGS.Shared.Game.Models.Card365.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Middlewares
{
    public class HandleCard365ExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleCard365ExceptionMiddleware(
            Func<string, IErrorMapper> mapper, ITgsLog log)
            : base(mapper, Consts.GameProvider.Card365, log, "365")
        {
        }

        protected override async Task CreateResponseAsync(
            HttpContext context,
            int statusCode,
            int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var res = new ErrorResponse
            {
                ErrorCode = (int)error,
                ErrorMessage = message
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(res);
        }
    }
}
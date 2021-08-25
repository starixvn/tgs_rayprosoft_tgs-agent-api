using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Middlewares
{
    public class HandleWMCasinoExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleWMCasinoExceptionMiddleware(
            Func<string, IErrorMapper> mapper, ITgsLog log) :
            base(mapper, Consts.GameProvider.WMCasino, log, "wmcasino")
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
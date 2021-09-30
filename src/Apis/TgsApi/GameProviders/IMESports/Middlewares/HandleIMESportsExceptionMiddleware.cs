// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.IMESports.Enums;
using TGS.Shared.Game.Models.IMESports.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Middlewares
{
    public sealed class HandleIMESportsExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleIMESportsExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, Consts.GameProvider.IMESports, log, "imesports")
        {
           
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode, Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var res = new BaseResponse
            {
                ErrorCode = (int)error,
                ErrorMessage = message,
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(res);
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.PragmaticPlay.Enums;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Middlewares
{
    public sealed class HandlePragmaticPlayExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandlePragmaticPlayExceptionMiddleware(
            Func<string, IErrorMapper> mapper, ITgsLog log)
            : base(mapper, Consts.GameProvider.PragmaticPlay, log, "pragmaticplay")
        {
        }

        protected override Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var response = new ErrorResponse
            {
                Error = (GameApiError)errorCode,
                Description = ErrorMapper.GetMessageByException(exception)
            };

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
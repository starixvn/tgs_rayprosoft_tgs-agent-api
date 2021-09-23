using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Middlewares
{
    public sealed class HandleJDBExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleJDBExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, Consts.GameProvider.JDB, log, Consts.GameProvider.JDB.ToLower())
        {
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var gameApiError = (GameApiStatus)errorCode;
            var response = new BaseActionResponse
            {
                Status = gameApiError.Description(),
                ErrorMessage = ErrorMapper.GetMessageByException(exception)
            };
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
            
        }
    }
}
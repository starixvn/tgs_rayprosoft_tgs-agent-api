using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Skywind.Common;
using TGS.Shared.Game.Models.Skywind.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Middlewares
{
    public sealed class HandleSkywindExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IGamePlayActions _action;

        public HandleSkywindExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action)
            : base(mapper, Consts.GameProvider.Skywind, log, "skywind")
        {
            _action = action;

        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {

            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);
            if (errorCode == 101 || errorCode == 900)
            {
                error = GameApiError.GameTokenExpired;
                message = GameApiError.GameTokenExpired.Description();
            }

            var response = new SkywindSingleWalletResponse()
            {
                ErrorCode = ((int)error),
                ErrorMsg = (!string.IsNullOrEmpty(error.Description())) ? error.Description() : message
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }

    }
}

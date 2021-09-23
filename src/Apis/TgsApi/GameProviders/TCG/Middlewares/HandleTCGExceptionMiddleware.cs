using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Enums;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Middlewares
{
    public sealed class HandleTCGExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        
        public HandleTCGExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, Consts.GameProvider.TCG, log, "tcg")
        {

        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var response = new TCGSingleWalletResponse()
            {
                Status = ((int)error),
                ErrorDesc = (!string.IsNullOrEmpty(error.Description())) ? error.Description() : message
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }
    }
}

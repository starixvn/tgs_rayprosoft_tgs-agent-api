using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.TF.Enums;
using TGS.Shared.Game.Models.TF.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TF.Middlewares
{
    public class HandleTFGamingExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleTFGamingExceptionMiddleware(
           Func<string, IErrorMapper> mapper,
           ITgsLog log)
           : base(mapper, Consts.GameProvider.TFGaming, log, "tf")
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
                ErrorCode = errorCode.ToString(),
                ErrorText = desc
            };

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync("");
        }
    }
}

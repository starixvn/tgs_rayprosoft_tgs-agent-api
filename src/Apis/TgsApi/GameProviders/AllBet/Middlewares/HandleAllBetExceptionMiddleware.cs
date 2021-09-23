using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.AllBet.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares
{
    public class HandleAllBetExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleAllBetExceptionMiddleware(
            Func<string, IErrorMapper> mapper, ITgsLog log)
            : base(mapper, Consts.GameProvider.AllBet, log, "allbet")
        {
        }

        protected override Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var response = new ErrorResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = ErrorMapper.GetMessageByException(exception)
            };

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}

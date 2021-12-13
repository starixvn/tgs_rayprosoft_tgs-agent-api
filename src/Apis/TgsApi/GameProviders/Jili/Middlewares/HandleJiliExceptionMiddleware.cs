using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using Tgs.Logging;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Middlewares
{
    public class HandleJiliExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private const string _GameProviderCode = Consts.GameProvider.Jili;

        public HandleJiliExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, _GameProviderCode, log, "jili")
        {
        }

        protected override Task CreateResponseAsync(
            HttpContext context,
            int statusCode,
            int errorCode,
            Exception exception)
        {
            var response = new JiliBaseResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = ErrorMapper.GetMessageByException(exception)
            };

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}

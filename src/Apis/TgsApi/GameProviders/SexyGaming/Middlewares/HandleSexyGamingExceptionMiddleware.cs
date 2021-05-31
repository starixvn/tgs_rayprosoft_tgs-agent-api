using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SexyGaming.Common;
using TGS.Shared.Game.Models.SexyGaming.Enums;
namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Middlewares
{
    public sealed class HandleSexyGamingExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IGamePlayActions _action;

        public HandleSexyGamingExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action)
            : base(mapper, Consts.GameProvider.SexyGaming, log, "sexygaming")
        {
            _action = action;

        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var response = new SexyGamingBaseResponse()
            {

                Status = ((int)error).ToString(),
                Desc = (!string.IsNullOrEmpty(error.Description())) ? error.Description() : message
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }

    }

}

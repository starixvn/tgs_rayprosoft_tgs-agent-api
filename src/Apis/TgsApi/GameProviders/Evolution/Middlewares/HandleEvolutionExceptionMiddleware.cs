using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Evolution.Commons;
using TGS.Shared.Game.Models.SexyGaming.Enums;
namespace TGS.Apis.TgsApi.GameProviders.Evolution.Middlewares
{
    public sealed class HandleEvolutionExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IGamePlayActions _action;

        public HandleEvolutionExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action)
            : base(mapper, Consts.GameProvider.SexyGaming, log, "evolution")
        {
            _action = action;

        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var response = new EvolutionBaseResponse()
            {
                Status = error.Description()
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }

    }

}

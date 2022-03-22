using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Enums;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Middlewares
{
    public sealed class HandleDreamGameExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleDreamGameExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, Consts.GameProvider.DreamGame, log, "dreamgame")
        {
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var value = await context.Request.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<DreamGameRequest>(value);

            var response = new DreamGameResponse()
            {
                CodeId = ((int)error),
                Token = request.Token
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }
    }
}

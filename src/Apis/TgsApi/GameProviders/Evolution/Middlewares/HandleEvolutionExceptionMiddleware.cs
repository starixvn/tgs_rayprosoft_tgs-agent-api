using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Logging;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Evolution.Commons;
using TGS.Shared.Game.Models.Evolution.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Middlewares
{
    public sealed class HandleEvolutionExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IGamePlayActions _action;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public HandleEvolutionExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action,
            IJsonSerializationProvider jsonSerializationProvider)
            : base(mapper, Consts.GameProvider.Evolution, log, "evolution")
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = error.Description();

            var body = await context.Request.ReadAsStringAsync();
            var baseRequest = _jsonSerializationProvider.DeserializeFromString<EvolutionBaseRequest>(body);

            var response = new EvolutionBaseResponse()
            {
                Status = message,
                Uuid = baseRequest.Uuid
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }

    }

}

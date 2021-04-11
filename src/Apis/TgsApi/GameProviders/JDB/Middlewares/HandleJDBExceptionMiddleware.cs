using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.JDB.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.JDB.Enums;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Middlewares
{
    public sealed class HandleJDBExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IGamePlayActions _action;
        private readonly IJDBActionModelMapper _map;

        private readonly Dictionary<Type, Func<HttpContext, GameApiError, string, Task<object>>> _responseGenerator;

        public HandleJDBExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action,
            IJDBActionModelMapper map)
            : base(mapper, Consts.GameProvider.Habanero, log, "habanero")
        {
            _action = action;
            _map = map;

            _responseGenerator = new();
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var requestType = context.Items.Get<Type>(Consts.ContextItems.RequestType, null);

            if (!_responseGenerator.TryGetValue(requestType, out var func))
            {
                throw new ArgumentException($"Not valid request type: {requestType}");
            }

            var response = await func(context, error, message);
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }
    }
}
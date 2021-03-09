using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Habanero.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Middlewares
{
    public sealed class HandleHabaneroExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IGamePlayActions _action;
        private readonly IHabaneroActionModelMapper _map;

        private readonly Dictionary<Type, Func<HttpContext, GameApiError, string, Task<object>>> _responseGenerator;

        public HandleHabaneroExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action,
            IHabaneroActionModelMapper map)
            : base(mapper, Consts.GameProvider.Habanero, log, "habanero")
        {
            _action = action;
            _map = map;

            _responseGenerator = new()
            {
                {typeof(AuthorizePlayerRequest), GetAuthorizePlayerResponseAsync},
                {typeof(FundTransferRequest), GetFundTransferResponseAsync}
            };
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

        #region Private Method

        private Task<object> GetAuthorizePlayerResponseAsync(HttpContext context, GameApiError error, string message)
        {
            var response = _map.Response(error, message);
            return Task.FromResult((object)response);
        }

        private async Task<object> GetFundTransferResponseAsync(HttpContext context, GameApiError error, string message)
        {
            GetBalanceResult result = null;

            if (error is not GameApiError.Uncategorized and not GameApiError.AuthorizationFailed)
            {
                var playerCode = context.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
                result = await _action.GetBalanceAsync(Consts.GameProvider.Habanero, playerCode);
            }

            var request = await context.Request.ReadAsObjectFromJsonAsync<FundTransferRequest>();
            var res = _map.Response(error, message, request, result);
            return res;
        }

        #endregion
    }
}
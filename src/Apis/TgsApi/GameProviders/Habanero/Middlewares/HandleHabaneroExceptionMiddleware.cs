using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.Habanero.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Middlewares
{
    public sealed class HandleHabaneroExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IHabaneroActionModelMapper _map;

        public HandleHabaneroExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IHabaneroActionModelMapper map)
            : base(mapper, Consts.GameProvider.Habanero, log, "habanero")
        {
            _map = map;
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var requestType = context.Items.Get<Type>(Consts.ContextItems.RequestType, null);

            if (requestType == typeof(AuthorizePlayerRequest))
            {
                var res = _map.Response(error, message);
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(res);
            }
            //context.Request.ReadAsObjectFromJsonAsync

            //var request = await context.Request.ReadAsObjectFromJsonAsync<BaseRequest>();

            //var res = new BaseResponse
            //{
            //    ErrorCode = (int)error,
            //    ErrorMessage = desc,
            //    PlayerCode = request.PlayerCode
            //};

            //context.Response.StatusCode = statusCode;
            //await context.Response.WriteAsJsonAsync(res);

            await Task.CompletedTask;
        }
    }
}
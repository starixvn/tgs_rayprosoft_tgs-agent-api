using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Middlewares
{
    public sealed class HandleHabaneroExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleHabaneroExceptionMiddleware(
            Func<string, IErrorMapper> mapper, 
            ITgsLog log) 
            : base(mapper, Consts.GameProvider.Habanero, log, "habanero")
        {
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            //var error = (GameApiError)errorCode;
            //var message = ErrorMapper.GetMessageByException(exception);
            //var desc = error.Description(message);

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

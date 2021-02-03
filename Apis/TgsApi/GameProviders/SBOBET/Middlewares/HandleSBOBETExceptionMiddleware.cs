// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Middlewares
{
    public sealed class HandleSBOBETExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleSBOBETExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, Consts.GameProvider.SBOBET, log, "sbo")
        {
           
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode, Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);
            var desc = error.Description(message);

            var request = await context.Request.ReadAsObjectFromJsonAsync<BaseRequest>();

            var res = new BaseResponse
            {
                ErrorCode = (int)error,
                ErrorMessage = desc,
                PlayerCode = request.PlayerCode
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(res);
        }
    }
}
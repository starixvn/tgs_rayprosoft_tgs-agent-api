// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Middlewares
{
    public sealed class HandleSBOBETExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        protected override string LoggerName => "sbo";

        public HandleSBOBETExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log)
            : base(mapper, Consts.GameProvider.SBOBET, log)
        {
           
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode, Exception exception)
        {
            var error = (GameApiError)errorCode;
            var desc = error.Description();

            var request = await context.Request.ReadFromJsonAsync<BaseRequest>();

            var res = new BaseResponse
            {
                ErrorCode = (int)error,
                ErrorMessage = desc.IsNotNullOrEmpty() ? desc : ErrorMapper.GetMessageByException(exception),
                PlayerCode = request.PlayerCode
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(res);
        }
    }
}
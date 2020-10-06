using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Filters;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Filters
{
    public sealed class HandleSBOErrorFilter : BaseHandleErrorFilter
    {
        protected override string LoggerName => "sbo";

        public HandleSBOErrorFilter(Func<string, IErrorMapper> mapper, ITgsLog log) :
            base(mapper, Consts.GameProvider.SBOBET, log)
        {
        }

        protected override async Task<IActionResult> CreateResponseAsync(ActionExecutedContext context, int statusCode, int errorCode)
        {
            var error = (GameApiError)errorCode;
            var desc = error.Description();

            var request = await context.HttpContext.Request.ReadFromJsonAsync<BaseRequest>();

            var res = new BaseResponse
            {
                ErrorCode = (int)error,
                ErrorMessage = desc.IsNotNullOrEmpty() ? desc : ErrorMapper.GetMessageByException(context.Exception),
                PlayerCode = request.PlayerCode
            };

            return new JsonResult(res) { StatusCode = statusCode };
        }
    }
}
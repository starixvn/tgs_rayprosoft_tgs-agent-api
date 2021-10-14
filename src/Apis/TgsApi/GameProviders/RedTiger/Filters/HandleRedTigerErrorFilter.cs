using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tgs.Logging;
using TGS.Core.Infrastructures.Filters;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger.Filters
{
    public sealed class HandleRedTigerErrorFilter : BaseHandleErrorFilter
    {
        protected override string LoggerName => "redtiger";

        public HandleRedTigerErrorFilter(Func<string, IErrorMapper> mapper, ITgsLog log) : 
            base(mapper, Consts.GameProvider.RedTiger, log)
        {

        }

        protected override Task<IActionResult> CreateResponseAsync(ActionExecutedContext context, int statusCode, int errorCode)
        {
            IActionResult r = new JsonResult(statusCode);
            return Task.FromResult(r);
        }
    }
}

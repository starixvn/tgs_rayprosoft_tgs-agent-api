using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Core.Infrastructures.Filters;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger.Filters
{
    public sealed class HandleRedTigerErrorFilter : BaseHandleErrorFilter
    {
        protected override string LoggerName => "redtiger";

        public HandleRedTigerErrorFilter(IErrorMapper error, ITgsLog log) : 
            base(error, log)
        {

        }

        protected override IActionResult CreateResponse(ActionExecutedContext context, int statusCode, int error)
        {
            return new JsonResult(statusCode);
        }
    }
}

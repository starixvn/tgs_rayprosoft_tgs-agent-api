using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Filters;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Interfaces;
using TGS.Core.Models.Messages;

namespace TGS.Core.Infrastructures.WebApiServices.Filters
{
    public sealed class HandleTgsErrorFilter : BaseHandleErrorFilter
    {
        protected override string LoggerName => nameof(HandleTgsErrorFilter);

        public HandleTgsErrorFilter(IErrorMapper errorMapper, ITgsLog log) :
            base(errorMapper, log)
        {

        }

        protected override IActionResult CreateResponse(ActionExecutedContext context, int statusCode, int error)
        {
            return new JsonResult(
                new TgsResponse
                {
                    Error = error,
                    ErrorMessage = context.Exception.GetMessageChain()
                });
        }
    }
}
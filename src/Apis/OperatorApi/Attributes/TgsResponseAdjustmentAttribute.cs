using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Apis.OperatorApi.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Attributes
{
    public class TgsResponseAdjustmentAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                return;

            if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new TgsResponse());
            }

            if (context.Result is ObjectResult result
                && result.Value is not TgsResponse)
            {
                var adjustResult = new ObjectResult(new SuccessResponse<object>
                {
                    Data = result.Value
                });

                adjustResult.StatusCode = result.StatusCode;
                adjustResult.Formatters = result.Formatters;

                context.Result = adjustResult;
            }
        }
    }
}

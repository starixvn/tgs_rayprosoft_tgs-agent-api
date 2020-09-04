using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TGS.Apis.BackendOffice.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {


        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                Console.WriteLine(context.Exception);
                //context.Result = new ObjectResult(exception.Value)
                //{
                //    StatusCode = exception.Status,
                //};
                context.ExceptionHandled = true;
            }
        }
    }
}

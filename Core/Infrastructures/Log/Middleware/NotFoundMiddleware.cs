using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace TGS.Core.Infrastructures.Log.Middleware
{
    public sealed class NotFoundMiddleware : IMiddleware
    {
        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);
            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                var result = new JsonResult(new { err = 900, errdesc = "The requested resource is not found" });
                var routeData = context.GetRouteData();
                var actionDescriptor = new ActionDescriptor();
                var actionContext = new ActionContext(context, routeData, actionDescriptor);
                await result.ExecuteResultAsync(actionContext);
            }
        }
    }
}

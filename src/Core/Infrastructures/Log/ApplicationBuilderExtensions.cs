using System.Linq;
using Microsoft.AspNetCore.Builder;
using TGS.Core.Infrastructures.Log.Middleware;

namespace TGS.Core.Infrastructures.Log
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiLogger(this IApplicationBuilder app, string logPrefix = "",
            string[] bypassRouting = null)
        {
            app.UseWhen(context => bypassRouting == null ||
                bypassRouting.All(r => !context.Request.Path.StartsWithSegments(r)), builder =>
                builder.UseMiddleware<ApiLoggerMiddleware>(logPrefix));

            return app;
        }
    }
}
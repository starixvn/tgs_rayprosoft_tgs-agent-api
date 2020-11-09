using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using TGS.Apis.OperatorApi.Middlewares;

namespace TGS.Apis.OperatorApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVerifyCredentialsMiddleware(this IApplicationBuilder app)
        {
            IList<string> bypassRouting = new[] { "/api/status", "/favicon.ico", "/swagger", "/api/auth" };

            app.UseWhen(context => bypassRouting.All(r => !context.Request.Path.StartsWithSegments(r)),
                builder => builder.UseMiddleware<VerifyCredentialsMiddleware>());

            return app;
        }
    }
}

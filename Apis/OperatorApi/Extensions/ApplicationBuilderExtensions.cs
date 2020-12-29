using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using TGS.Apis.OperatorApi.Middlewares;

namespace TGS.Apis.OperatorApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static readonly IList<string> BypassRouting = new[]
            {"/health", "/favicon.ico", "/swagger", "/api/auth"};

        public static IApplicationBuilder UseVerifyJwtTokenMiddlewares(this IApplicationBuilder app)
        {
            app.UseWhen(context => BypassRouting.All(r => !context.Request.Path.StartsWithSegments(r)),
                builder => builder
                    .UseMiddleware<VerifyCredentialsMiddleware>()
                    .UseMiddleware<VerifyPlayerTokenMiddleware>());

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/player"),
                builder => builder.UseMiddleware<VerifyCaptchaMiddleware>());

            return app;
        }
    }
}
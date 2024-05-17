using System.Linq;
using Microsoft.AspNetCore.Builder;
using TGS.Apis.AgentApi.Middlewares;

namespace TGS.Apis.AgentApi.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseVerifyBrandTokenMiddlewares(this IApplicationBuilder app,
        string[] bypassRouting)
    {
        var bypass = bypassRouting.Concat(new[] { "/api/auth" }).Distinct().ToArray();

        _ = app.UseBrandCertificate();

        _ = app.UseWhen(context => bypass.All(r => !context.Request.Path.StartsWithSegments(r)),
            builder => builder
                .UseMiddleware<VerifyCredentialsMiddleware>()
                .UseMiddleware<MaintenanceMiddleware>()
                .UseMiddleware<VerifyAgentTokenMiddleware>());

        return app;
    }
}
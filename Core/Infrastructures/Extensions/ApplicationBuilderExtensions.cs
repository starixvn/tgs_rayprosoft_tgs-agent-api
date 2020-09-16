using Microsoft.AspNetCore.Builder;
using TGS.Core.Infrastructures.Log.Middleware;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNotFoundHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NotFoundMiddleware>();
        }

        public static IApplicationBuilder UseApiLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLoggerMiddleware>();
        }
    }
}

using Microsoft.AspNetCore.Builder;
using TGS.Infrastructures.Log.Middleware;

namespace TGS.Infrastructures.Extensions
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

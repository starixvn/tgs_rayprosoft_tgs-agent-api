using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Middlewares;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MethodMiddlewareExtension
    {

        public static IApplicationBuilder UseBGMethodMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MethodMiddleware>();
        }

    }
}

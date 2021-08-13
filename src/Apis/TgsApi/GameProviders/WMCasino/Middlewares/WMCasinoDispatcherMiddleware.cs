using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Middlewares
{
    public class WMCasinoDispatcherMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var data = await context.Request.ReadAsStringAsync();
            var decode = WebUtility.UrlDecode(data);

            var form = context.Request.Form;
            if (!form.TryGetValue("cmd", out var cmd) && string.IsNullOrEmpty(cmd))
            {
                throw new ArgumentException("Missing cmd parameter");
            }

            await next(context);
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares
{
    public sealed class AuthorizeAllBetMiddleware : IMiddleware
    {
        private static readonly string _Routing = "/api/ab";

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.ToUriComponent();
            var idx = path.IndexOf(_Routing, StringComparison.OrdinalIgnoreCase);
            var apiPath = path.Substring(idx).Replace(_Routing, string.Empty);

            //var json = await context.Request.ReadAsStringAsync();
            //var doc = JsonDocument.Parse(json);
            await next(context);
        }
    }
}
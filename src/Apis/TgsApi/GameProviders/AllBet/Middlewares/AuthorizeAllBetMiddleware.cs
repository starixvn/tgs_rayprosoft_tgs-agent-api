using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares
{
    public sealed class AuthorizeAllBetMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var json = await context.Request.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            await next(context);
        }
    }
}
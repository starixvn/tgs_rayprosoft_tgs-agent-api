using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Middlewares
{
    public sealed class AuthorizePragmaticPlayMiddleware : IMiddleware
    {
        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var coll = await context.Request.ReadFormAsync();

            await next(context).ConfigureAwait(false);
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Json;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Middlewares
{
    public sealed class AuthorizeJDBMiddleware : IMiddleware
    {
        private readonly IJsonSerializationProvider _json;

        public AuthorizeJDBMiddleware(
            IJsonSerializationProvider json)
        {
            _json = json;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var jsonValue = context.Request.Form;
            
            await next(context);
        }
    }
}
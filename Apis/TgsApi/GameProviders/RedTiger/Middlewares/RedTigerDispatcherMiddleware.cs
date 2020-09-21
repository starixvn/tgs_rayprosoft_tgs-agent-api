using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger.Middlewares
{
    public class RedTigerDispatcherMiddleware
    {
        private readonly IJsonSerializationProvider _json;
        private readonly RequestDelegate _next;

        public RedTigerDispatcherMiddleware(IJsonSerializationProvider json, RequestDelegate next)
        {
            _json = json;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                await _next(context);
                return;
            }

            var model = await context.Request.ReadFromJsonAsync<Request>();

            if (model.Action.IsNullOrEmpty())
            {
                throw new Exception("No Action is found");
            }

            if (model.Action.IgnoreCaseEquals("bet"))
            {
                var o = new { action = model.Action, value = 333 };
                var json = _json.SerializeToString(o);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                context.Request.Body = stream;

                context.Request.Path = $"{context.Request.PathBase.Value}/api/bet/places";
                await _next(context);
                return;
            }
            else if (model.Action.IgnoreCaseEquals("error"))
            { 
                var o = new { action = model.Action, value = 333 };
                var json = _json.SerializeToString(o);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                context.Request.Body = stream;

                context.Request.Path = $"{context.Request.PathBase.Value}/api/bet/error";
                await _next(context);
                return;
            }

            await _next(context);
        }
    }

    public record Request
    {
        public string Action { get; init; }
    }
}
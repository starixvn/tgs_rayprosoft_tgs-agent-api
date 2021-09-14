using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Middlewares
{
    public class Card365DispatcherMiddleware : IMiddleware
    {
        private static readonly IReadOnlyDictionary<string, string> _Routes = new Dictionary<string, string>
        {
            { "getBalance", "/api/365/balance" },
            { "bet", "/api/365/bet" },
            { "settle", "/api/365/settle" },
            { "voidGame", "/api/365/void" },
            { "cancelBet", "/api/365/cancelbet" }
        };

        private static readonly RecyclableMemoryStreamManager _RecyclableMemoryStreamManager =
            new RecyclableMemoryStreamManager();

        private readonly JsonSerializerOptions _jsonSerializeOptions;

        public Card365DispatcherMiddleware()
        {
            _jsonSerializeOptions = new JsonSerializerOptions { WriteIndented = false };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var form = context.Request.Form;
            if (!form.TryGetValue("key", out var key) && string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Missing key parameter");
            }

            if (!form.TryGetValue("message", out var message) && string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Missing message parameter");
            }

            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

            if (!items.TryGetValue("action", out var value))
            {
                throw new ArgumentException("Missing action parameter");
            }

            var action = value?.ToString() ?? string.Empty;
            if (!_Routes.TryGetValue(action, out var route))
            {
                await next(context);
                return;
            }

            items["key"] = key.ToString();
            items["path"] = context.Request.Path;

            var json = JsonSerializer.Serialize(items, _jsonSerializeOptions);

            var stream = _RecyclableMemoryStreamManager.GetStream(Encoding.UTF8.GetBytes(json));
            context.Request.Body = stream;
            context.Request.ContentType = MediaTypeNames.Application.Json;
            context.Request.Path = route;

            await next(context);
        }
    }
}
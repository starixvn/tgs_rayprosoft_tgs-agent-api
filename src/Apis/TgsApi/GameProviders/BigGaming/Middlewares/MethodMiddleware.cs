using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using TGS.Shared.Game.Models.BigGaming.Common;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Middlewares
{
    public class MethodMiddleware
    {
        private readonly RequestDelegate _next;

        public MethodMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestContent;
            var request = context.Request;
            var url = context.Request.Path.Value;

            if (url.Contains("/api/bg/entrance", StringComparison.InvariantCultureIgnoreCase))
            {
                request.EnableBuffering();

                var streamManager = new RecyclableMemoryStreamManager();

                await using var stream = streamManager.GetStream();
                await request.Body.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using var sr = new StreamReader(stream);
                var body = await sr.ReadToEndAsync();

                request.Body.Seek(0, SeekOrigin.Begin);

                var response = JsonSerializer.Deserialize<BGBaseRequest>(body);
                context.Request.Path = response.Method.ToLower() switch
                {
                    "open.operator.user.balance" => "/api/bg/balance",
                    "open.operator.order.transfer" => "/api/bg/bet",
                    "open.operator.calc.transfer" => "/api/bg/settle",
                    _ => url
                };

            }

            await _next(context);


        }
    }
}

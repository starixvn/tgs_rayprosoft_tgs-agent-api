using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace TGS.Apis.TgsApi.GameProviders.TF.Middlewares
{
    public class TFDispatcherMiddleware : IMiddleware
    {
        private static readonly RecyclableMemoryStreamManager _RecyclableMemoryStreamManager =
            new RecyclableMemoryStreamManager();

        private static readonly IReadOnlyDictionary<string, string> _Routes = new Dictionary<string, string>
        {
            { "Place bet", "/tf/api/placebet" },
            { "Settlement", "/tf/api/settle" },
            { "Unsettlement", "/tf/api/unsettle"},            
            { "Loss", "/tf/api/settle" }
        };

        public TFDispatcherMiddleware()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Request.EnableBuffering();

            var streamReader = new StreamReader(context.Request.Body);
            var body = await streamReader.ReadToEndAsync();

            if (body.Contains("description"))
            {
                var items = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                var path = _Routes[items["description"].ToString()];

                var json = JsonSerializer.Serialize(items);
                var stream = _RecyclableMemoryStreamManager.GetStream(Encoding.UTF8.GetBytes(json));

                context.Request.Body = stream;
                context.Request.ContentType = MediaTypeNames.Application.Json;
                context.Request.Path = path;
            }
            await next(context);
        }
    }
}

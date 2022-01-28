using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Middlewares
{
    public class GzipRequestDecompressionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Headers.ContainsKey(HeaderNames.ContentEncoding) &&
                context.Request.Headers[HeaderNames.ContentEncoding] == "gzip")
            {
                context.Request.Headers.Remove(HeaderNames.ContentEncoding);

                var memoryStream = new MemoryStream();

                await using var stream = new GZipStream(context.Request.Body, CompressionMode.Decompress);
                await stream.CopyToAsync(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = memoryStream;
                context.Request.Headers[HeaderNames.ContentLength] = memoryStream.Length.ToString();
            }

            await next(context);
        }
    }
}

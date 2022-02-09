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
            if (context.Request.Method != HttpMethods.Get &&
                context.Request.Headers.ContainsKey(HeaderNames.ContentEncoding) &&
                context.Request.Headers[HeaderNames.ContentEncoding] == "gzip")
            {
                context.Request.Headers.Remove(HeaderNames.ContentEncoding);

                var stream = new GZipStream(context.Request.Body, CompressionMode.Decompress, true);
                context.Request.Body = stream;
            }

            await next(context);
        }
    }
}

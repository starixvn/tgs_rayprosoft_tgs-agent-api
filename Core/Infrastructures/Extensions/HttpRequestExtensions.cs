using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<string> ReadAsStringAsync(this HttpRequest request)
        {
            request.EnableBuffering();

            var streamManager = new RecyclableMemoryStreamManager();

            await using var stream = streamManager.GetStream();
            await request.Body.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(stream);
            var body = await sr.ReadToEndAsync();

            request.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }
    }
}

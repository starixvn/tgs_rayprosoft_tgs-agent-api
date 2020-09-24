using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public class AuthenticateMiddleware
    {
        private readonly IHashProvider _hashProvider;
        private readonly ITgsQueries<Licensee> _licenseeQueries;
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public AuthenticateMiddleware(IHashProvider hashProvider, ITgsQueries<Licensee> licenseeQueries, RequestDelegate next)
        {
            _hashProvider = hashProvider;
            _licenseeQueries = licenseeQueries;
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                await _next(context);
                return;
            }

            var clientId = context.Request.Headers["clientid"];
            var time = context.Request.Headers["time"];
            var authCode = context.Request.Headers["authcode"];

            var licensee = await _licenseeQueries.GetSingleAsync(p => p.ClientId == clientId);
            if (licensee == null)
            {
                return;
            }

            if (licensee.Status != Status.Active)
            {
                return;
            }

            var hashedCode = _hashProvider.Hash(
                licensee.ClientId + licensee.SecretKey + time,
                HashAlgorithmKind.Sha512, 
                HashStringKind.Base64,
                Encoding.UTF8);

            if (!hashedCode.Equals(authCode))
            {
                return;
            }

            var reqMessage = await GetRequestBodyAsync(context);

            var s = context.Request.Form["LicenseeId"];


            await _next(context);
        }

        private async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            requestStream.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(requestStream);
            var message = await sr.ReadToEndAsync();
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            return message;
        }

    }
}
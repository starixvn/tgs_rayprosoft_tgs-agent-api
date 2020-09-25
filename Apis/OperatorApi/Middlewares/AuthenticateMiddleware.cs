using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using TGS.Apis.OperatorApi.Controllers.Messages.Players;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public class AuthenticateMiddleware : IMiddleware
    {
        private readonly IHashProvider _hashProvider;
        private readonly IRedisCacheManager _cacheManager;
        private readonly ITgsQueries<Licensee> _licenseeQueries;
        private readonly IJsonSerializationProvider _json;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public AuthenticateMiddleware(IHashProvider hashProvider, ITgsQueries<Licensee> licenseeQueries, IJsonSerializationProvider json, IRedisCacheManager cacheManager)
        {
            _hashProvider = hashProvider;
            _licenseeQueries = licenseeQueries;
            _json = json;
            _cacheManager = cacheManager;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                await next(context);
                return;
            }
            
            var clientId = (string)context.Request.Headers["clientid"];
            var time = (string)context.Request.Headers["time"];
            var authCode = (string)context.Request.Headers["authcode"];

            if (clientId.IsNullOrEmpty() || time.IsNullOrEmpty() || authCode.IsNullOrEmpty())
            {
                throw new InvalidOperationException("Invalid request.");
            }

            //TODO: Need to cache
            var licensee = await _licenseeQueries.GetSingleAsync(p => 
                p.ClientId == clientId);

            if (licensee == null)
            {
                throw new InvalidOperationException("Licensee not found");
            }

            if (licensee.Status != Status.Active)
            {
                throw new InvalidOperationException("Licensee is not active");
            }

            var hashedCode = _hashProvider.Hash(
                licensee.ClientId + licensee.SecretKey + time,
                HashAlgorithmKind.Sha512, 
                HashStringKind.Base64,
                Encoding.UTF8);

            if (!hashedCode.Equals(authCode))
            {
                throw new InvalidOperationException("Authenticate licensee failed");
            }

            var reqMessage = await GetRequestBodyAsync(context);
            var req =_json.DeserializeFromString<BaseRequest>(reqMessage) ;

            if (req.action.IgnoreCaseEquals("create") || req.action.IgnoreCaseEquals("login"))
            {
                context.Request.Headers.Add("licenseeId", licensee.Id.ToString());
            }

            await next(context);
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

        public class RequestBody
        {
            public int Id { get; set; }
            public string Result { get; set; }
            public Exception Exception { get; set; }
        }
    }
}
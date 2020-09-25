using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IO;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class VerifyCredentialsMiddleware
    {
        private readonly IHashProvider _hashProvider;        
        private readonly IJsonSerializationProvider _json;
        private readonly RequestDelegate _next;

        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public VerifyCredentialsMiddleware(
            IHashProvider hashProvider,             
            IJsonSerializationProvider json,            
            RequestDelegate next)
        {
            _hashProvider = hashProvider;
            _json = json;
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context, 
            ILicenseeOperations licenseeOperations, 
            IStatusValidator statusValidator)
        {                     
            var clientId = (string)context.Request.Headers["clientid"];
            var timestamp = (string)context.Request.Headers["timestamp"];
            var authCode = (string)context.Request.Headers["authcode"];

            if (clientId.IsNullOrEmpty() || timestamp.IsNullOrEmpty() || authCode.IsNullOrEmpty())
            {
                throw new ArgumentException("Required header parameters are missing");
            }

            //TODO: Need to cache
            var licenseeId = await licenseeOperations.GetLicenseeIdByClientIdAsync(clientId);
            if (licenseeId == Guid.Empty)
            {
                throw new ClientIdNotFoundException($"Lincesee is not found with client id: {clientId}");
            }

            await statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var secretKey = await licenseeOperations.GetLicenseeSecretKeyAsync(licenseeId);
           
            var hashedCode = _hashProvider.Hash(
                $"{clientId}{secretKey}{timestamp}",
                HashAlgorithmKind.Sha512, 
                HashStringKind.Hex,
                Encoding.UTF8);

            if (!hashedCode.Equals(authCode))
            {
                throw new InvalidCredentialsException($"Fail to authorize licensee with client id: {clientId}");
            }

            //var reqMessage = await GetRequestBodyAsync(context);
            //var req =_json.DeserializeFromString<BaseRequest>(reqMessage) ;

            //if (req.action.IgnoreCaseEquals("create") || req.action.IgnoreCaseEquals("login"))
            //{
            //    context.Request.Headers.Add("licenseeId", licensee.Id.ToString());
            //}

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

        public class RequestBody
        {
            public int Id { get; set; }
            public string Result { get; set; }
            public Exception Exception { get; set; }
        }
    }
}
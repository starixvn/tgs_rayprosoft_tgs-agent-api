using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using TGS.Core.Infrastructures.Extensions;
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
        private readonly RequestDelegate _next;

        public VerifyCredentialsMiddleware(
            IHashProvider hashProvider,
            RequestDelegate next)
        {
            _hashProvider = hashProvider;
            _next = next;
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
            
            context.Items.Add(Consts.ContextItems.LicenseeId, licenseeId);

            await _next(context);
        }
    }
}
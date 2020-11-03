using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using ContextItems = TGS.Core.Models.Consts.ContextItems;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class VerifyCredentialsMiddleware
    {
        private readonly IHashProvider _hashProvider;
        private readonly RequestDelegate _next;

        private const double TimestampLimitSeconds = 60 * 2;
        private const string Iso8601Format = @"yyyy-MM-dd\THH:mm:ssK";

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

            if (!DateTimeOffset.TryParseExact(timestamp, Iso8601Format, CultureInfo.InvariantCulture, styles: DateTimeStyles.None, out var time))
            {
                throw new ArgumentException("Invalid format of timestamp");
            }

            if (time.AddSeconds(TimestampLimitSeconds) < DateTimeOffset.Now)
            {
                throw new InvalidCredentialsException("The timestamp is exceeded the limit time");
            }

            var licenseeId = await licenseeOperations.GetLicenseeIdByClientIdAsync(clientId);
            if (licenseeId == Guid.Empty)
            {
                throw new ClientIdNotFoundException($"Licensee is not found with client id: {clientId}");
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
            
            context.Items.Add(ContextItems.LicenseeId, licenseeId);

            await _next(context);
        }
    }
}
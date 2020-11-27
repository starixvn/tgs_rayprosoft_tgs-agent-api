using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class VerifyCredentialsMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyCredentialsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
            IBrandOperations brandOperations,
            IStatusValidator statusValidator,
            IJwtTokenHandler handler)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var values))
            {
                throw new MissingTokenException("Missing Authorization Header");
            }

            var authorization = values.FirstOrDefault();
            if (authorization == null || !authorization.StartsWith("Bearer", true, null))
            {
                throw new MissingTokenException("Missing Authorization Bearer Header");
            }

            var token = authorization[6..].Trim();
            var jwt = handler.ReadToken(token);

            var sub = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var currency = jwt.Claims.FirstOrDefault(x => x.Type == Consts.ClaimName.Currency)?.Value;

            if (sub.IsNullOrEmpty() || !Guid.TryParse(sub, out var licenseeId))
            {
                throw new InvalidCredentialsException("Licensee doesn't exist");
            }

            await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);
            var secretKey = await brandOperations.GetBrandSecretKeyAsync(licenseeId);
            var signature = $"{licenseeId}:{currency}:{secretKey}";

            handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Consts.Site,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signature)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                },
                exception => throw new InvalidCredentialsException(exception.Message),
                _ => throw new InvalidCredentialsException("token expired"));

            context.Items.Add(Core.Models.Consts.ContextItems.BrandId, licenseeId);

            await _next(context);
        }
    }
}
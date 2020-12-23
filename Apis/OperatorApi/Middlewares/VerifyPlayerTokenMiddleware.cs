using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class VerifyPlayerTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyPlayerTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IExpirationValidator expirationValidator,
            IJwtTokenHandler handler)
        {
            var attribute = context.GetEndpoint()?.Metadata?.GetMetadata<AuthorizePlayerTokenAttribute>();

            if (attribute == null)
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers.TryGetValue("Token", out var value) ? value.FirstOrDefault() : null;

            if (token.IsNullOrEmpty())
            {
                if (!attribute.AllowNullOrEmpty)
                {
                    throw new MissingTokenException(Resources.Player.MissingPlayerToken);
                }

                await _next(context);
                return;
            }

            var jwt = handler.ReadToken(token);

            var sub = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (sub.IsNullOrEmpty())
            {
                throw new MissingTokenException(Resources.Player.MissingPlayerToken);
            }

            var data = await tokenProvider.DecryptAsync<PlayerTokenData>(sub);
            context.Items.Add(Core.Models.Consts.ContextItems.PlayerToken, data);

            var playerCode = data.PlayerCode;
            await expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, data.PlatformType);

            if (!attribute.NeedAuthorize)
            {
                await _next(context);
                return;
            }

            var licenseeId = await playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var secretKey = await brandOperations.GetBrandSecretKeyAsync(licenseeId);

            var jti = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var payload = hashProvider.Hash($"{sub}:{jti}");
            var signature = $"{payload}:{secretKey}";

            handler.ValidateToken(token, new()
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
                _ => throw new PlayerLoginExpiredException(playerCode));

            await _next(context);
        }
    }
}
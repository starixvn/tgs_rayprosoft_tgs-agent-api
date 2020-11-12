using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models.Messages.Players;
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

        public VerifyPlayerTokenMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IPlayerOperations playerOperations,
            ILicenseeOperations licenseeOperations,
            IExpirationValidator expirationValidator)
        {
            var attribute = context.GetEndpoint()?.Metadata?.GetMetadata<AuthorizePlayerTokenAttribute>();

            if (attribute == null)
            {
                await _next(context);
                return;
            }

            var request = await context.Request.ReadAsObjectFromJsonAsync<BaseTokenRequest>();

            var token = request?.PlayerToken;
            if (token.IsNullOrEmpty())
            {
                if (!attribute.AllowNullOrEmpty)
                {
                    throw new MissingTokenException("token is empty.");
                }

                await _next(context);
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var sub = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (sub.IsNullOrEmpty())
            {
                throw new MissingTokenException("token is empty.");
            }

            var data = tokenProvider.Decrypt<PlayerTokenData>(sub);
            context.Items.Add(Core.Models.Consts.ContextItems.PlayerToken, data);

            var playerCode = data.PlayerCode;
            await expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, data.PlatformType);
            
            if (!attribute.NeedAuthorize)
            {
                await _next(context);
                return;
            }

            var licenseeId = await playerOperations.GetLicenseeIdByPlayerCodeAsync(playerCode);
            var secretKey = await licenseeOperations.GetLicenseeSecretKeyAsync(licenseeId);

            var jti = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var payload = hashProvider.Hash($"{sub}:{jti}");
            var signature = $"{payload}:{secretKey}";

            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Consts.Site,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signature)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);
            }
            catch (SecurityTokenExpiredException)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
            catch (Exception e)
            {
                throw new InvalidCredentialsException(e.Message);
            }

            await _next(context);
        }
    }
}